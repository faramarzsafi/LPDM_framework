from copy import copy
from typing import List, TypeVar

from jmetal.core.algorithm import EvolutionaryAlgorithm
from jmetal.core.operator import Mutation
from jmetal.core.problem import Problem
from jmetal.util.constraint_handling import overall_constraint_violation_degree
from jmetal.util.evaluator import Evaluator, SequentialEvaluator
from jmetal.util.generator import Generator, RandomGenerator
from jmetal.util.termination_criterion import TerminationCriterion
from jmetal.algorithm.singleobjective.evolution_strategy import EvolutionStrategy
from thesis.output import print_solutions

S = TypeVar("S")
R = TypeVar("R")

"""
.. module:: evolutionary_algorithm
   :platform: Unix, Windows
   :synopsis: Implementation of Evolutionary Algorithms.

.. moduleauthor:: Antonio J. Nebro <antonio@lcc.uma.es>, Antonio Benítez-Hidalgo <antonio.b@uma.es>
"""


class ES(EvolutionStrategy):
    def __init__(
        self,
        problem: Problem,
        mu: int,
        lambda_: int,
        elitist: bool,
        mutation: Mutation,
        termination_criterion: TerminationCriterion,
        population_generator: Generator = RandomGenerator(),
        population_evaluator: Evaluator = SequentialEvaluator(),
    ):
        super(ES, self).__init__(problem=problem, mu=mu, lambda_=lambda_, elitist=elitist, mutation=mutation,
                                 termination_criterion=termination_criterion, population_generator=population_generator,
                                 population_evaluator=population_evaluator)
        self.mu = mu
        self.lambda_ = lambda_
        self.elitist = elitist

        self.mutation_operator = mutation

        self.population_generator = population_generator
        self.population_evaluator = population_evaluator

        self.termination_criterion = termination_criterion
        self.observable.register(termination_criterion)
        self.iteration_no = 0


    def create_initial_solutions(self) -> List[S]:
        return [self.population_generator.new(self.problem) for _ in range(self.population_size)]

    def evaluate(self, solution_list: List[S]):
        self.iteration_no += 1
        return self.population_evaluator.evaluate(solution_list, self.problem)

    def stopping_condition_is_met(self) -> bool:
        return self.termination_criterion.is_met

    def selection(self, population: List[S]) -> List[S]:
        return population

    def reproduction(self, population: List[S]) -> List[S]:
        offspring_population = []
        for solution in population:
            for j in range(int(self.lambda_ / self.mu)):
                new_solution = copy(solution)
                offspring_population.append(self.mutation_operator.execute(new_solution))

        return offspring_population

    def replacement(self, population: List[S], offspring_population: List[S]) -> List[S]:
        population_pool = []

        if self.elitist:
            population_pool = population
            population_pool.extend(offspring_population)
        else:
            population_pool.extend(offspring_population)

        population_pool.sort(key=lambda s: (overall_constraint_violation_degree(s), s.objectives[0]))

        new_population = []
        for i in range(self.mu):
            new_population.append(population_pool[i])

        return new_population

    def get_result(self) -> R:
        return self.solutions[0]

    def get_name(self) -> str:
        return "Elitist evolution Strategy"

    def update_progress(self) -> None:
        super(ES, self).update_progress()
        self.problem.compute_diversities(self.solutions)
