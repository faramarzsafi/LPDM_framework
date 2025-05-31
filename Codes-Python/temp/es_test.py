from jmetal.operator.mutation import PermutationSwapMutation
from jmetal.util.termination_criterion import StoppingByTime
from thesis.output import print_problem_diversities
from thesis.problem.permutation_problem import ackley_n4_fcn_problem
from thesis.problem.problem1 import Problem1
from thesis.algorithm.es import ES

if __name__ == "__main__":
    number_of_variables = 99
    problem = ackley_n4_fcn_problem(number_of_variables)

    algorithm = ES(
        problem=problem,
        mu=1,
        lambda_=20,
        mutation=PermutationSwapMutation(1.0 / problem.number_of_variables),
        elitist=False,
        termination_criterion=StoppingByTime(100),
    )

    algorithm.run()
    result = algorithm.get_result()

    print(f"Algorithm: {algorithm.get_name()}")
    print(f"Solution: {result.variables}")
    print(f"The shortest path length: {result.objectives[0]}")
    print(f"Computing time: {algorithm.total_computing_time}")
    print(f"Number of evaluations: {algorithm.evaluations}")
    print_problem_diversities(problem)
