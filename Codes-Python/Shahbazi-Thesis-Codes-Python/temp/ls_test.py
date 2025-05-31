from jmetal.operator.mutation import PermutationSwapMutation
from jmetal.util.termination_criterion import StoppingByEvaluations
from thesis.output import print_problem_diversities
from thesis.problem.permutation_problem import ackley_n4_fcn_problem
from thesis.problem.problem1 import Problem1
from thesis.algorithm.ls import LS

if __name__ == "__main__":
    number_of_variables = 10
    problem = ackley_n4_fcn_problem(number_of_variables)

    print("number_of_variables: ", problem.number_of_variables)

    algorithm = LS(
        problem=problem,
        mutation=PermutationSwapMutation(1.0 / problem.number_of_variables),
        termination_criterion=StoppingByEvaluations(max_evaluations=2500),
    )

    algorithm.run()
    result = algorithm.get_result()

    print("Algorithm: {}".format(algorithm.get_name()))
    print("Problem: {}".format(problem.get_name()))
    print("Solution: {}".format(result.variables))
    print("Fitness: {}".format(result.objectives[0]))
    print("Computing time: {}".format(algorithm.total_computing_time))
    print_problem_diversities(problem)
