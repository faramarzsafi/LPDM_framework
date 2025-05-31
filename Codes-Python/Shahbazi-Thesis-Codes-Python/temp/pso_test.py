from jmetal.algorithm.multiobjective.smpso import SMPSO
from jmetal.operator import PolynomialMutation, PermutationSwapMutation
from jmetal.problem import Srinivas
from jmetal.util.archive import CrowdingDistanceArchive
from jmetal.util.solution import (
    print_function_values_to_file,
    print_variables_to_file,
    read_solutions,
)
from jmetal.util.termination_criterion import StoppingByEvaluations
from thesis.algorithm.pso import PSO
from thesis.output import print_problem_diversities
from thesis.problem.permutation_problem import ackley_n4_fcn_problem

if __name__ == "__main__":
    number_of_variables = 99
    problem = ackley_n4_fcn_problem(number_of_variables)

    max_evaluations = 25000
    algorithm = PSO(
        problem=problem,
        swarm_size=100,
        mutation=PermutationSwapMutation(1.0 / problem.number_of_variables),
        leaders=CrowdingDistanceArchive(100),
        termination_criterion=StoppingByEvaluations(max_evaluations=max_evaluations),
    )

    algorithm.run()
    front = algorithm.get_result()

    # Save results to file
    # print_function_values_to_file(front, "FUN." + algorithm.label)
    # print_variables_to_file(front, "VAR." + algorithm.label)

    print(f"Algorithm: {algorithm.get_name()}")
    print(f"Problem: {problem.get_name()}")
    print(f"Computing time: {algorithm.total_computing_time}")
    print(f"Front: {front}")
    print_problem_diversities(problem)
