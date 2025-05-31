from jmetal.core.problem import FloatProblem
from jmetal.core.solution import FloatSolution
from thesis.diversity.salleh_et_al import salleh_et_al
from thesis.diversity.ursem import ursem
from thesis.diversity.osuna_enciso_et_al import osuna_enciso_et_al
from thesis.benchmark.cec import ackley_fcn_real


class real_problem(FloatProblem):
    def __init__(self, number_of_variables):
        self.number_of_variables = number_of_variables
        self.number_of_objectives = 1
        self.number_of_constraints = 0
        self.lower_bound = [0 for _ in range(number_of_variables)]
        self.upper_bound = [number_of_variables for _ in range(number_of_variables)]
        self.diversities = [ursem(self.number_of_variables), salleh_et_al(self.number_of_variables),
                            osuna_enciso_et_al(self.number_of_variables)]
        self.name = "AckleyFCN"

    def compute_diversities(self, solution_list):
        for i in range(len(self.diversities)):
            self.diversities[i].compute(solution_list)

    def evaluate(self, solution: FloatSolution) -> FloatSolution:
        fitness = ackley_fcn_real(solution)

        solution.objectives[0] = fitness

        return solution

    def get_name(self):
        return self.name
