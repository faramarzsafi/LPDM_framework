from jmetal.core.solution import PermutationSolution
from jmetal.problem import TSP
from thesis.problem.permutation_problem import permutation_problem


class tsp1(TSP, permutation_problem):
    def __init__(self, instance):
        super(tsp, self).__init__(instance=instance)
        self.renew()

    def evaluate(self, solution: PermutationSolution) -> PermutationSolution:
        fitness = 0
        # for i in range(self.number_of_variables):
        #     solution.variables[i] = i

        for i in range(self.number_of_variables - 1):
            x = solution.variables[i]
            y = solution.variables[i + 1]

            fitness += self.distance_matrix[x][y]

        first_city, last_city = solution.variables[0], solution.variables[-1]
        fitness += self.distance_matrix[first_city][last_city]

        solution.objectives[0] = fitness
        # print(fitness, solution.variables)

        return solution


