import copy
import random
import threading
import time
from abc import ABC
from typing import List, TypeVar

from jmetal.config import store
from jmetal.core.algorithm import Algorithm
from jmetal.core.operator import Mutation, Selection
from jmetal.core.problem import Problem
from jmetal.core.solution import Solution
from jmetal.util.comparator import Comparator
from jmetal.util.evaluator import Evaluator
from jmetal.util.generator import Generator
from jmetal.util.termination_criterion import TerminationCriterion

S = TypeVar("S")
R = TypeVar("R")


class TabuSearch(Algorithm[S, R], threading.Thread, ABC):
    def __init__(
        self,
        problem: Problem,
        mutation: Mutation,
        population_size: int,
        selection: Selection,
        termination_criterion: TerminationCriterion = store.default_termination_criteria,
        comparator: Comparator = store.default_comparator,
        population_evaluator: Evaluator = store.default_evaluator,
        population_generator: Generator = store.default_generator,
    ):
        super(TabuSearch, self).__init__()
        self.comparator = comparator
        self.problem = problem
        self.mutation = mutation
        self.termination_criterion = termination_criterion
        self.observable.register(termination_criterion)
        self.population_size: population_size
        # crossover: Crossover,
        self.selection = selection,
        self.population_evaluator = population_evaluator
        self.population_generator = population_generator

    #

    def create_initial_solutions(self) -> List[S]:
        self.solutions.append(self.problem.create_solution())
        return self.solutions

    def evaluate(self, population: List[S]) -> List[S]:
        return self.population_evaluator.evaluate(population, self.problem)

    def stopping_condition_is_met(self) -> bool:
        return self.termination_criterion.is_met

    def init_progress(self) -> None:
        self.evaluations = 0

    def step(self) -> None:
        mutated_solution = copy.deepcopy(self.solutions[0])
        mutated_solution: Solution = self.mutation.execute(mutated_solution)
        mutated_solution = self.evaluate([mutated_solution])[0]

        result = self.comparator.compare(mutated_solution, self.solutions[0])
        if result == -1:
            self.solutions[0] = mutated_solution
        elif result == 1:
            pass
        else:
            if random.random() < 0.5:
                self.solutions[0] = mutated_solution

    def update_progress(self) -> None:
        self.evaluations += 1

        observable_data = self.get_observable_data()
        self.observable.notify_all(**observable_data)

    def get_observable_data(self) -> dict:
        ctime = time.time() - self.start_computing_time
        return {
            "PROBLEM": self.problem,
            "EVALUATIONS": self.evaluations,
            "SOLUTIONS": self.get_result(),
            "COMPUTING_TIME": ctime,
        }

    def get_result(self) -> R:
        return self.solutions[0]

    def get_name(self) -> str:
        return "LS"
