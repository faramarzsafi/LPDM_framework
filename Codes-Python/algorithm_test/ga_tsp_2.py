import datetime

from jmetal.core.solution import PermutationSolution
from jmetal.operator import BinaryTournamentSelection, BestSolutionSelection
from jmetal.operator.crossover import PMXCrossover, CXCrossover
from jmetal.operator.mutation import PermutationSwapMutation, ScrambleMutation
from jmetal.problem import TSP
from jmetal.util.comparator import MultiComparator
from jmetal.util.density_estimator import CrowdingDistance
from jmetal.util.ranking import FastNonDominatedRanking
from jmetal.util.termination_criterion import StoppingByEvaluations, StoppingByTime
from thesis.diversity.osuna_enciso_et_al import osuna_enciso_et_al
from thesis.diversity.ours import ours
from thesis.diversity.salleh_et_al import salleh_et_al
from thesis.diversity.ursem import ursem
from thesis.output import print_problem_diversities, print_problem_ours, print_results
from thesis.premutation.premutation import generate_coefficients_of_integer, generate_permutation, factorial
from thesis.problem.permutation_problem import ackley_fcn_problem, ackley_n4_fcn_problem, permutation_problem
from thesis.algorithm.ga import GA
from thesis.problem.tsp import tsp

if __name__ == "__main__":
    from os import getcwd

    cwd = getcwd()
    print(cwd)
    # problem = tsp(instance="resources/TSP_instances/kroA100.tsp")
    problem = tsp(distance_filename="resources/p01_d.txt")

    print("number_of_variables: ", problem.number_of_variables)
    now = datetime.datetime.now()
    print("Start time: {}".format(now))

    algorithm = GA(
        problem=problem,
        population_size=100,
        offspring_population_size=10,
        # mutation=ScrambleMutation(3.0 / problem.number_of_variables),
        mutation=ScrambleMutation(0.3),
        # crossover=PMXCrossover(0.7),
        crossover=CXCrossover(0.8),
        # selection=BinaryTournamentSelection(
        #     MultiComparator([FastNonDominatedRanking.get_comparator(), CrowdingDistance.get_comparator()])
        # ),
        selection=BestSolutionSelection(),

        # termination_criterion=StoppingByEvaluations(max_evaluations=5000),
        termination_criterion=StoppingByTime(max_seconds=60),
    )

    algorithm.run()
    result = algorithm.get_result()
    time_name = now.strftime("%Y%m%d-%H%M%S")
    print_results(algorithm, problem, result, filename=time_name, output_path="D:\\Personal\\Master\\THESIS\\Tests\\Compare")
    print_problem_diversities(problem, filename=time_name, output_path="D:\\Personal\\Master\\THESIS\\Tests\\Compare")
    print_problem_ours(problem, filename=time_name, output_path="D:\\Personal\\Master\\THESIS\\Tests\\Compare")

