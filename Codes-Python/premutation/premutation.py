from typing import List, TypeVar

S = TypeVar("S")
factorials = None


def fact(n):
    if n == 0:
        return 1
    return n * fact(n - 1)


def factorial(number_of_variables):
    global factorials
    if factorials is None:
        factorials = []
    existing_factorials_count = len(factorials)
    for i in range(existing_factorials_count, number_of_variables + 1):
        factorials.append(fact(i))
    return factorials[number_of_variables]


def generate_permutation(number_of_variables, representation: int) -> List[S]:
    coefficients = generate_coefficients_of_integer(number_of_variables, representation)
    jobs = generate_permutation_by_coefficients(number_of_variables, coefficients)
    return jobs


def generate_coefficients_of_integer(number_of_variables, representation: int):
    if number_of_variables == 0:
        return None
    coefficients = []
    i = 1
    n = representation
    while i <= number_of_variables:
        coefficients.append(n % i)
        n = int(n / i)
        i += 1
    return coefficients


def generate_permutation_by_coefficients(number_of_variables, coefficients) -> List[S]:
    if len(coefficients) != number_of_variables:
        raise Exception("Invalid coefficients count")
    jobs = []
    for i in range(number_of_variables):
        jobs.append(i)
    coefficient_list = coefficients[::-1]
    permutation_list = []
    permutation_list.append(coefficient_list[0])
    jobs.pop(permutation_list[0])
    coefficient_list.pop(0)
    while len(coefficient_list) > 0:
        index_of_job = coefficient_list[0]
        job = jobs[index_of_job]
        jobs.pop(index_of_job)
        coefficient_list.pop(0);
        permutation_list.append(job)
    return permutation_list


def generate_integer_representation(permutation: List[S]):
    coefficients = generate_coefficients_of_permutation(permutation)
    result = 0
    for i in range(len(permutation)):
        result += factorial(i) * coefficients[i]
    return result


def generate_coefficients_of_permutation(permutation: List[S]):
    solution = []
    for i in range(len(permutation)):
        solution.append(i)
    coefficients = []
    permutation_copy = permutation.copy()
    while len(permutation_copy) > 0:
        index = solution.index(permutation_copy[0])
        coefficients.append(index)
        solution.pop(index)
        permutation_copy.pop(0)
    coefficients.reverse()
    return coefficients


number_of_variables = 10
coefficients = generate_coefficients_of_integer(number_of_variables, 42)
print(coefficients)
permutation = generate_permutation_by_coefficients(number_of_variables, coefficients)
print(permutation)
integer = generate_integer_representation(permutation)
print(integer)
print(factorials)
