from jmetal.config import store
from jmetal.core.algorithm import EvolutionaryAlgorithm
from jmetal.algorithm.singleobjective.genetic_algorithm import GeneticAlgorithm
from typing import List, TypeVar

from jmetal.core.operator import Mutation, Crossover, Selection
from jmetal.core.problem import Problem
from jmetal.util.evaluator import Evaluator
from jmetal.util.generator import Generator
from jmetal.util.termination_criterion import TerminationCriterion
from thesis.output import print_solutions

S = TypeVar("S")
R = TypeVar("R")


class GA(GeneticAlgorithm):
    def __init__(self,
                 problem: Problem,
                 population_size: int,
                 offspring_population_size: int,
                 mutation: Mutation,
                 crossover: Crossover,
                 selection: Selection,
                 termination_criterion: TerminationCriterion = store.default_termination_criteria,
                 population_generator: Generator = store.default_generator,
                 population_evaluator: Evaluator = store.default_evaluator,
                 ):
        super(GA, self).__init__(problem, population_size, offspring_population_size, mutation, crossover, selection,
                                 termination_criterion, population_generator, population_evaluator)
        self.iteration_no = 0

    def evaluate(self, solution_list: List[S]) -> List[S]:
        self.iteration_no += 1
        return super(GA, self).evaluate(solution_list)

    def update_progress(self) -> None:
        super(GA, self).update_progress()
        self.problem.compute_diversities(self.solutions)

