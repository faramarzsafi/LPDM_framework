import math
import random
import re

from jmetal.core.problem import PermutationProblem
from jmetal.core.solution import PermutationSolution
from diversity.cheng import cheng
from diversity.li_et_al import li_et_al
from diversity.osuna_enciso_et_al import osuna_enciso_et_al
from diversity.ours import ours
from diversity.salleh_et_al import salleh_et_al
from diversity.shi import shi
from diversity.tilahun import tilahun
from diversity.ursem import ursem
from diversity.wang_et_al import wang_et_al
from diversity.wineberg_oppacher import wineberg_oppacher
from diversity.zhao_et_al import zhao_et_al
from diversity.zhu_et_al import zhu_et_al
from problem.permutation_problem import permutation_problem


class tsp_problem(permutation_problem):
    def __init__(self, number_of_variables=0):
        super(tsp_problem, self).__init__(number_of_variables)
        self.best_costs = []
        self.costs_avg = []
        self.diversities = [
            ours(self.number_of_variables),
            ursem(self.number_of_variables),
            li_et_al(self.number_of_variables),
            cheng(self.number_of_variables),
            tilahun(self.number_of_variables),
            salleh_et_al(self.number_of_variables),
            wineberg_oppacher(self.number_of_variables),
            zhu_et_al(self.number_of_variables),
            shi(self.number_of_variables),
            wang_et_al(self.number_of_variables),
            zhao_et_al(self.number_of_variables),
            osuna_enciso_et_al(self.number_of_variables),
        ]

    def compute_diversities(self, solution_list, best_cost, cost_avg):
        self.best_costs.append(best_cost)
        self.costs_avg.append(cost_avg)
        for i in range(len(self.diversities)):
            self.diversities[i].compute(solution_list)
