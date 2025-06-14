"""

 public StandardPSO2011(DoubleProblem problem, int objectiveId, int swarmSize, int maxIterations,
                         int numberOfParticlesToInform, SolutionListEvaluator<DoubleSolution> evaluator) {
    this.problem = problem;
    this.swarmSize = swarmSize;
    this.maxIterations = maxIterations;
    this.numberOfParticlesToInform = numberOfParticlesToInform;
    this.evaluator = evaluator;
    this.objectiveId = objectiveId;

    weight = 1.0 / (2.0 * Math.log(2)); //0.721;
    c = 1.0 / 2.0 + Math.log(2); //1.193;
    changeVelocity = -0.5 ;

    fitnessComparator = new ObjectiveComparator<DoubleSolution>(objectiveId);
    findBestSolution = new BestSolutionSelection<DoubleSolution>(fitnessComparator);

    localBest = new DoubleSolution[swarmSize];
    neighborhoodBest = new DoubleSolution[swarmSize];
    speed = new double[swarmSize][problem.numberOfVariables()];
    randomParticle = new DoubleSolution[swarmSize] ;

    positionInSwarm = new GenericSolutionAttribute<DoubleSolution, Integer>();

    randomGenerator = JMetalRandom.getInstance() ;
    randomGenerator.setRandomGenerator(new ExtendedPseudoRandomGenerator(new JavaRandomGenerator()));

    bestFoundParticle = null;
    neighborhood = new AdaptiveRandomNeighborhood<DoubleSolution>(swarmSize, this.numberOfParticlesToInform);
  }

  /**
   * Constructor
   *
   * @param problem
   * @param swarmSize
   * @param maxIterations
   * @param numberOfParticlesToInform
   * @param evaluator
   */
  public StandardPSO2011(DoubleProblem problem, int swarmSize, int maxIterations,
                         int numberOfParticlesToInform, SolutionListEvaluator<DoubleSolution> evaluator) {
    this(problem, 0, swarmSize, maxIterations, numberOfParticlesToInform, evaluator);
  }

  @Override
  public void initProgress() {
    iterations = 1;
  }

  @Override
  public void updateProgress() {
    iterations += 1;
  }

  @Override
  public boolean isStoppingConditionReached() {
    return iterations >= maxIterations;
  }

  @Override
  public List<DoubleSolution> createInitialSwarm() {
    List<DoubleSolution> swarm = new ArrayList<>(swarmSize);

    DoubleSolution newSolution;
    for (int i = 0; i < swarmSize; i++) {
      newSolution = problem.createSolution();
      positionInSwarm.setAttribute(newSolution, i);
      swarm.add(newSolution);
    }

    return swarm;
  }

  @Override
  public List<DoubleSolution> evaluateSwarm(List<DoubleSolution> swarm) {
    swarm = evaluator.evaluate(swarm, problem);

    return swarm;
  }

  @Override
  public void initializeLeader(List<DoubleSolution> swarm) {
    for (int i = 0; i < swarm.size(); i++) {
      neighborhoodBest[i] = getNeighborBest(i);
    }
  }

  @Override
  public void initializeParticlesMemory(List<DoubleSolution> swarm) {
    for (int i = 0; i < swarm.size(); i++) {
      localBest[i] = (DoubleSolution) swarm.get(i).copy();
    }
  }

  @Override
  public void initializeVelocity(List<DoubleSolution> swarm) {
    for (int i = 0; i < swarmSize; i++) {
      DoubleSolution particle = swarm.get(i);
      for (int j = 0; j < problem.numberOfVariables(); j++) {
        Bounds<Double> bounds = particle.getBounds(j) ;
        speed[i][j] = (randomGenerator.nextDouble(
                bounds.getLowerBound() - particle.variables().get(0),
                bounds.getUpperBound() - particle.variables().get(0)));
      }
    }
  }

  @Override
  public void updateVelocity(List<DoubleSolution> swarm) {
    for (int i = 0; i < swarmSize; i++) {
      DoubleSolution particle = swarm.get(i);
      DoubleSolution gravityCenter = problem.createSolution();

      if (this.localBest[i] != this.neighborhoodBest[i]) {
        for (int j = 0; j < particle.variables().size(); j++) {
          double value = particle.variables().get(j) +
                  c * (localBest[i].variables().get(j) +
                          neighborhoodBest[i].variables().get(j) - 2 *
                          particle.variables().get(j)) / 3.0;

          gravityCenter.variables().set(j, value);
        }
      } else {
        for (int j = 0; j < particle.variables().size(); j++) {
          double g  = particle.variables().get(j) +
                  c * (localBest[i].variables().get(j) - particle.variables().get(j)) / 2.0;

          gravityCenter.variables().set(j, g);
        }
      }

      double radius = 0;
      radius = SolutionUtils.distanceBetweenSolutionsInObjectiveSpace(gravityCenter, particle);

      double[] random = ((ExtendedPseudoRandomGenerator)randomGenerator.getRandomGenerator()).randSphere(problem.numberOfVariables());

      randomParticle[i] = problem.createSolution() ;
      for (int j = 0; j < particle.variables().size(); j++) {
        randomParticle[i].variables().set(j, gravityCenter.variables().get(j) + radius * random[j]);
      }

      for (int var = 0; var < particle.variables().size(); var++) {
        speed[i][var] =
                weight * speed[i][var] + randomParticle[i].variables().get(var) - particle.variables().get(var);
      }
    }
  }

  @Override
  public void updatePosition(List<DoubleSolution> swarm) {
    for (int i = 0; i < swarmSize; i++) {
      DoubleSolution particle = swarm.get(i);
      for (int j = 0; j < particle.variables().size(); j++) {
        particle.variables().set(j, particle.variables().get(j) + speed[i][j]);

        Bounds<Double> bounds = problem.variableBounds().get(j) ;
        Double lowerBound = bounds.getLowerBound() ;
        Double upperBound = bounds.getUpperBound() ;
        if (particle.variables().get(j) < lowerBound) {
          particle.variables().set(j, lowerBound);
          speed[i][j] = changeVelocity * speed[i][j];
        }
        if (particle.variables().get(j) > upperBound) {
          particle.variables().set(j, upperBound);
          speed[i][j] = changeVelocity * speed[i][j];
        }
      }
    }
  }

  @Override
  public void perturbation(List<DoubleSolution> swarm) {
  }

  @Override
  public void updateLeaders(List<DoubleSolution> swarm) {
    for (int i = 0; i < swarm.size(); i++) {
      neighborhoodBest[i] = getNeighborBest(i);
    }

    DoubleSolution bestSolution = findBestSolution.execute(swarm);

    if (bestFoundParticle == null) {
      bestFoundParticle = (DoubleSolution) bestSolution.copy();
    } else {
      if (bestSolution.objectives()[objectiveId] == bestFoundParticle.objectives()[0]) {
        neighborhood.recompute();
      }
      if (bestSolution.objectives()[objectiveId] < bestFoundParticle.objectives()[0]) {
        bestFoundParticle = (DoubleSolution) bestSolution.copy();
      }
    }
  }

  @Override
  public void updateParticlesMemory(List<DoubleSolution> swarm) {
    for (int i = 0; i < swarm.size(); i++) {
      if ((swarm.get(i).objectives()[objectiveId] < localBest[i].objectives()[0])) {
        localBest[i] = (DoubleSolution) swarm.get(i).copy();
      }
    }
  }

  @Override
  public DoubleSolution getResult() {
    return bestFoundParticle;
  }

  private DoubleSolution getNeighborBest(int i) {
    DoubleSolution bestLocalBestSolution = null;

    for (DoubleSolution solution : neighborhood.getNeighbors(getSwarm(), i)) {
      int solutionPositionInSwarm = positionInSwarm.getAttribute(solution);
      if ((bestLocalBestSolution == null) || (bestLocalBestSolution.objectives()[0]
              > localBest[solutionPositionInSwarm].objectives()[0])) {
        bestLocalBestSolution = localBest[solutionPositionInSwarm];
      }
    }

    return bestLocalBestSolution ;
  }

  /* Getters */
  public double[][]getSwarmSpeedMatrix() {
    return speed ;
  }

  public DoubleSolution[] getLocalBest() {
    return localBest ;
  }

  @Override public String getName() {
    return "SPSO11" ;
  }

  @Override public String getDescription() {
    return "Standard PSO 2011" ;
  }
}
"""