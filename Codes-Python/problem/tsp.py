import math
import random
import re

from jmetal.core.problem import PermutationProblem
from jmetal.core.solution import PermutationSolution
from thesis.problem.permutation_problem import permutation_problem


class tsp(permutation_problem):
    """Class representing TSP Problem."""

    def __init__(self, tsplib_filename: str = None, distance_filename: str = None):
        super(tsp, self).__init__()

        if tsplib_filename is not None:
            distance_matrix, number_of_cities = self.__read_from_tsplib_file(tsplib_filename)
        elif distance_filename is not None:
            distance_matrix, number_of_cities = self.__read_from_distance_file(distance_filename)
        else:
            raise Exception("No data")

        self.distance_matrix = distance_matrix

        self.obj_directions = [self.MINIMIZE]
        self.number_of_variables = number_of_cities
        self.number_of_objectives = 1
        self.number_of_constraints = 0
        self.renew()

    def __read_from_tsplib_file(self, filename: str):
        """
        This function reads a TSP Problem instance from a file.

        :param filename: File which describes the instance.
        :type filename: str.
        """

        if filename is None:
            raise FileNotFoundError("Filename can not be None")

        with open(filename) as file:
            lines = file.readlines()
            data = [line.lstrip() for line in lines if line != ""]

            dimension = re.compile(r"[^\d]+")
            for item in data:
                if item.startswith("DIMENSION"):
                    dimension = int(dimension.sub("", item))
                    break

            c = [-1.0] * (2 * dimension)

            for item in data:
                if item[0].isdigit():
                    x = [x.strip() for x in item.split(" ")]
                    j, city_a, city_b = int(x[0]), float(x[1]), float(x[2])
                    c[2 * (j - 1)] = city_a
                    c[2 * (j - 1) + 1] = city_b

            matrix = [[-1] * dimension for _ in range(dimension)]

            for k in range(dimension):
                matrix[k][k] = 0

                for j in range(k + 1, dimension):
                    dist = math.sqrt((c[k * 2] - c[j * 2]) ** 2 + (c[k * 2 + 1] - c[j * 2 + 1]) ** 2)
                    dist = round(dist)
                    matrix[k][j] = dist
                    matrix[j][k] = dist

            return matrix, dimension

    def __read_from_distance_file(self, filename: str):

        if filename is None:
            raise FileNotFoundError("Filename can not be None")

        with open(filename) as file:
            lines = file.readlines()
            data = []
            for line in lines:
                line = line.strip()
                if line != "":
                    data.append(line)

            dimension = len(data)

            matrix = [[-1] * dimension for _ in range(dimension)]

            for i in range(dimension):
                items = data[i].split()
                for j in range(len(items)):
                    matrix[i][j] = float(items[j])

            return matrix, dimension

    def evaluate(self, solution: PermutationSolution) -> PermutationSolution:
        fitness = 0

        for i in range(self.number_of_variables - 1):
            x = solution.variables[i]
            y = solution.variables[i + 1]

            fitness += self.distance_matrix[x][y]

        first_city, last_city = solution.variables[0], solution.variables[-1]
        fitness += self.distance_matrix[first_city][last_city]

        solution.objectives[0] = fitness

        return solution

    def create_solution(self) -> PermutationSolution:
        new_solution = PermutationSolution(
            number_of_variables=self.number_of_variables, number_of_objectives=self.number_of_objectives
        )
        new_solution.variables = random.sample(range(self.number_of_variables), k=self.number_of_variables)

        return new_solution

    @property
    def number_of_cities(self):
        return self.number_of_variables

    def get_name(self):
        return "Symmetric TSP"
