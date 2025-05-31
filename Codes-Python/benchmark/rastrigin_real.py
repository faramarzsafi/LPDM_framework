import math

from jmetal.core.solution import FloatSolution
from thesis.diversity.ursem import ursem
from thesis.problem.real_problem import real_problem


class rastrigin(real_problem):
    def __init__(self, number_of_variables: int = 10):
        super(rastrigin, self).__init__()
        self.obj_directions = [self.MINIMIZE]
        self.obj_labels = ["f(x)"]

        self.lower_bound = [-5.12 for _ in range(number_of_variables)]
        self.upper_bound = [5.12 for _ in range(number_of_variables)]

        FloatSolution.lower_bound = self.lower_bound
        FloatSolution.upper_bound = self.upper_bound
        self.name = "Rastrigin"

    def evaluate(self, solution: FloatSolution) -> FloatSolution:
        a = 10.0
        result = a * solution.number_of_variables
        x = solution.variables

        for i in range(solution.number_of_variables):
            result += x[i] * x[i] - a * math.cos(2 * math.pi * x[i])

        solution.objectives[0] = result

        return solution



