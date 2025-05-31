from jmetal.operator import ScrambleMutation
from jmetal.util.termination_criterion import StoppingByTime

from thesis.algorithm.sa import SA
from thesis.output import print_problem_diversities
from thesis.problem.permutation_problem import ackley_n4_fcn_problem
from thesis.problem.problem1 import Problem1

if __name__ == "__main__":
    number_of_variables = 99
    problem = ackley_n4_fcn_problem(number_of_variables)

    algorithm = SA(
        problem=problem,
        mutation=ScrambleMutation(probability=1.0 / problem.number_of_variables),
        termination_criterion=StoppingByTime(max_seconds=100),
    )

    algorithm.run()
    result = algorithm.get_result()

    print(f"Algorithm: {algorithm.get_name()}")
    print(f"Solution: {result.variables}")
    print(f"The shortest path length:  {str(result.objectives[0])}")
    print(f"Computing time: {str(algorithm.total_computing_time)}")
    print(f"Problem evaluations: {str(algorithm.evaluations)}")
    print_problem_diversities(problem)
