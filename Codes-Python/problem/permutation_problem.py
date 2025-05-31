import random

from diversity.cheng import cheng
from diversity.diversity import global_number_of_variables
from diversity.li_et_al import li_et_al
from diversity.osuna_enciso_et_al import osuna_enciso_et_al
from diversity.ours import ours
from diversity.shi import shi
from diversity.tilahun import tilahun
from diversity.ursem import ursem
from diversity.salleh_et_al import salleh_et_al
from benchmark.cec import ackley_fcn, ackley_n4_fcn
from jmetal.core.problem import PermutationProblem
from jmetal.core.solution import PermutationSolution
from diversity.wang_et_al import wang_et_al
from diversity.wineberg_oppacher import wineberg_oppacher
from diversity.zhao_et_al import zhao_et_al
from diversity.zhu_et_al import zhu_et_al


class permutation_problem(PermutationProblem):

    def __init__(self, number_of_variables=0):
        super(permutation_problem, self).__init__()
        self.number_of_variables = global_number_of_variables if number_of_variables == 0 else number_of_variables

        self.number_of_objectives = 1
        self.number_of_constraints = 0
        self.diversities = []
        self.lower_bound = [0 for n in range(self.number_of_variables)]
        self.upper_bound = [number_of_variables - 1 for n in range(self.number_of_variables)]
        self.ours_clusters = []

        self.renew()

    def renew(self):
        self.diversities = [
            ours(self.number_of_variables),
            ursem(self.number_of_variables),
            wineberg_oppacher(self.number_of_variables),
            zhu_et_al(self.number_of_variables),
            shi(self.number_of_variables),
            wang_et_al(self.number_of_variables),
            li_et_al(self.number_of_variables),
            cheng(self.number_of_variables),
            tilahun(self.number_of_variables),
            salleh_et_al(self.number_of_variables),
            zhao_et_al(self.number_of_variables),
            osuna_enciso_et_al(self.number_of_variables),
        ]
        self.lower_bound = [0 for n in range(self.number_of_variables)]
        self.upper_bound = [self.number_of_variables - 1 for n in range(self.number_of_variables)]

    def evaluate(self, solution: PermutationSolution) -> PermutationSolution:
        fitness = ackley_n4_fcn(solution)

        solution.objectives[0] = fitness

        return solution

    def create_solution(self) -> PermutationSolution:
        new_solution = PermutationSolution(
            number_of_variables=self.number_of_variables, number_of_objectives=self.number_of_objectives
        )
        new_solution.variables = random.sample(range(self.number_of_variables), k=self.number_of_variables)

        return new_solution

    def get_name(self):
        return "Problem 1"

    def compute_diversities(self, solution_list):
        for i in range(len(self.diversities)):
            self.diversities[i].compute(solution_list)


class ackley_fcn_problem(permutation_problem):
    def evaluate(self, solution: PermutationSolution) -> PermutationSolution:
        fitness = ackley_fcn(solution)

        solution.objectives[0] = fitness

        return solution

    def get_name(self):
        return "AckleyFCN"


class ackley_n4_fcn_problem(permutation_problem):
    def evaluate(self, solution: PermutationSolution) -> PermutationSolution:
        fitness = ackley_n4_fcn(solution)

        solution.objectives[0] = fitness

        return solution

    def get_name(self):
        return "AckleyN4FCN"
