import threading
import time
from abc import ABC, abstractmethod
from typing import Generic, List, TypeVar

from jmetal.config import store
from jmetal.core.algorithm import Algorithm
from jmetal.core.problem import Problem
from jmetal.logger import get_logger

logger = get_logger(__name__)

S = TypeVar("S")
R = TypeVar("R")


class ParticleSwarmOptimization(Algorithm[S, R], ABC):
    def __init__(self, problem: Problem[S], swarm_size: int):
        super(ParticleSwarmOptimization, self).__init__()
        self.problem = problem
        self.swarm_size = swarm_size

    @abstractmethod
    def initialize_velocity(self, swarm: List[S]) -> None:
        pass

    @abstractmethod
    def initialize_particle_best(self, swarm: List[S]) -> None:
        pass

    @abstractmethod
    def initialize_global_best(self, swarm: List[S]) -> None:
        pass

    @abstractmethod
    def update_velocity(self, swarm: List[S]) -> None:
        pass

    @abstractmethod
    def update_particle_best(self, swarm: List[S]) -> None:
        pass

    @abstractmethod
    def update_global_best(self, swarm: List[S]) -> None:
        pass

    @abstractmethod
    def update_position(self, swarm: List[S]) -> None:
        pass

    @abstractmethod
    def perturbation(self, swarm: List[S]) -> None:
        pass

    def get_observable_data(self) -> dict:
        return {
            "PROBLEM": self.problem,
            "EVALUATIONS": self.evaluations,
            "SOLUTIONS": self.get_result(),
            "COMPUTING_TIME": time.time() - self.start_computing_time,
        }

    def init_progress(self) -> None:
        self.evaluations = self.swarm_size

        self.initialize_velocity(self.solutions)
        self.initialize_particle_best(self.solutions)
        self.initialize_global_best(self.solutions)

        observable_data = self.get_observable_data()
        self.observable.notify_all(**observable_data)

    def step(self):
        self.update_velocity(self.solutions)
        self.update_position(self.solutions)
        self.perturbation(self.solutions)
        self.solutions = self.evaluate(self.solutions)
        self.update_global_best(self.solutions)
        self.update_particle_best(self.solutions)

    def update_progress(self) -> None:
        self.evaluations += self.swarm_size

        observable_data = self.get_observable_data()
        self.observable.notify_all(**observable_data)

    @property
    def label(self) -> str:
        return f"{self.get_name()}.{self.problem.get_name()}"
