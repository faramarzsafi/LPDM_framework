from jmetal.operator import BinaryTournamentSelection
from jmetal.operator.crossover import SBXCrossover
from jmetal.operator.mutation import PolynomialMutation
from jmetal.util.termination_criterion import StoppingByEvaluations
from thesis.output import print_problem_diversities
from thesis.benchmark.ackley_fcn_real import ackley_fcn_real_problem
from thesis.algorithm.ga import GA

if __name__ == "__main__":
    # problem = Rastrigin1(10)
    number_of_variables = 99
    # problem = ackley_n4_fcn_real_problem(number_of_variables)
    problem = ackley_fcn_real_problem(number_of_variables)
    algorithm = GA(
        problem=problem,
        population_size=100,
        offspring_population_size=1,
        mutation=PolynomialMutation(1.0 / problem.number_of_variables, 20.0),
        crossover=SBXCrossover(0.9, 5.0),
        selection=BinaryTournamentSelection(),
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
