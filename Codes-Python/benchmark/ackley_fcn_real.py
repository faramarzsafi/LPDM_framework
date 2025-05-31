from jmetal.core.problem import FloatProblem
from jmetal.core.solution import FloatSolution
from thesis.diversity.salleh_et_al import salleh_et_al
from thesis.diversity.ursem import ursem
from thesis.benchmark.cec import ackley_fcn_real
from thesis.problem.real_problem import real_problem


class ackley_fcn_real_problem(real_problem):
    def evaluate(self, solution: FloatSolution) -> FloatSolution:
        fitness = ackley_fcn_real(solution)

        solution.objectives[0] = fitness

        return solution

    def get_name(self):
        return "AckleyFCN"
