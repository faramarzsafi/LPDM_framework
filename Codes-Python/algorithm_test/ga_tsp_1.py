import datetime

from jmetal.core.solution import PermutationSolution
from jmetal.operator import BinaryTournamentSelection
from jmetal.operator.crossover import PMXCrossover
from jmetal.operator.mutation import PermutationSwapMutation
from jmetal.problem import TSP
from jmetal.util.comparator import MultiComparator
from jmetal.util.density_estimator import CrowdingDistance
from jmetal.util.ranking import FastNonDominatedRanking
from jmetal.util.termination_criterion import StoppingByEvaluations, StoppingByTime
from thesis.diversity.osuna_enciso_et_al import osuna_enciso_et_al
from thesis.diversity.ours import ours
from thesis.diversity.salleh_et_al import salleh_et_al
from thesis.diversity.ursem import ursem
from thesis.output import print_problem_diversities, print_problem_ours
from thesis.premutation.premutation import generate_coefficients_of_integer, generate_permutation, factorial
from thesis.problem.permutation_problem import ackley_fcn_problem, ackley_n4_fcn_problem, permutation_problem
from thesis.algorithm.ga import GA
from thesis.problem.tsp import tsp

if __name__ == "__main__":
    from os import getcwd

    cwd = getcwd()
    print(cwd)
    # problem = tsp(instance="resources/TSP_instances/kroA100.tsp")
    problem = tsp(tsplib_filename="resources/new.tsp")

    print("number_of_variables: ", problem.number_of_variables)
    print("Start time: {}".format(datetime.datetime.now()))

    algorithm = GA(
        problem=problem,
        population_size=100,
        offspring_population_size=50,
        mutation=PermutationSwapMutation(3.0 / problem.number_of_variables),
        crossover=PMXCrossover(0.7),
        selection=BinaryTournamentSelection(
            MultiComparator([FastNonDominatedRanking.get_comparator(), CrowdingDistance.get_comparator()])
        ),
        # termination_criterion=StoppingByEvaluations(max_evaluations=5000),
        termination_criterion=StoppingByTime(max_seconds=180),
    )
    # max = factorial(10)
    # min = max
    # for i in range(max):
    #     x = generate_permutation(10, i)
    #     p = PermutationSolution(10, 1)
    #     p.variables = x
    #     problem.evaluate(p)
    #     if p.objectives[0] < min:
    #         min = p.objectives[0]
    # print("Min ", min)

    algorithm.run()
    result = algorithm.get_result()

    print("Algorithm: {}".format(algorithm.get_name()))
    print("Problem: {}".format(problem.get_name()))
    print("Solution: {}".format(result.variables))
    print("Fitness: {}".format(result.objectives[0]))
    print("Computing time: {}".format(algorithm.total_computing_time))
    print_problem_diversities(problem)
    # print_problem_ours(problem)

