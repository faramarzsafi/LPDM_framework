from jmetal.operator import BinaryTournamentSelection
from jmetal.operator.crossover import PMXCrossover
from jmetal.operator.mutation import PermutationSwapMutation
from jmetal.problem import TSP
from jmetal.util.comparator import MultiComparator
from jmetal.util.density_estimator import CrowdingDistance
from jmetal.util.ranking import FastNonDominatedRanking
from jmetal.util.termination_criterion import StoppingByEvaluations
from thesis.diversity.osuna_enciso_et_al import osuna_enciso_et_al
from thesis.diversity.ours import ours
from thesis.diversity.salleh_et_al import salleh_et_al
from thesis.diversity.ursem import ursem
from thesis.output import print_problem_diversities
from thesis.problem.job_scheduling import job_scheduling
from thesis.problem.permutation_problem import ackley_fcn_problem, ackley_n4_fcn_problem, permutation_problem
from thesis.algorithm.ga import GA
from thesis.problem.tsp1 import tsp

if __name__ == "__main__":
    from os import getcwd

    cwd = getcwd()
    print(cwd)
    problem = job_scheduling(filename="resources/taillard_benchmarks/tai100_10.txt")

    print("number_of_variables: ", problem.number_of_variables)

    algorithm = GA(
        problem=problem,
        population_size=100,
        offspring_population_size=100,
        mutation=PermutationSwapMutation(1.0 / problem.number_of_variables),
        crossover=PMXCrossover(0.8),
        selection=BinaryTournamentSelection(
            MultiComparator([FastNonDominatedRanking.get_comparator(), CrowdingDistance.get_comparator()])
        ),
        termination_criterion=StoppingByEvaluations(max_evaluations=25000),
    )

    algorithm.run()
    result = algorithm.get_result()

    print("Algorithm: {}".format(algorithm.get_name()))
    print("Problem: {}".format(problem.get_name()))
    print("Solution: {}".format(result.variables))
    print("Fitness: {}".format(result.objectives[0]))
    print("Computing time: {}".format(algorithm.total_computing_time))
    print_problem_diversities(problem)
