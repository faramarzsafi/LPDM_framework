class diversity:
    def __init__(self):
        self.results = []
        self.name = ""

    def add_result(self, result):
        self.results.append(result)

    def clear_results(self):
        self.results.clear()

    def compute(self) -> float:
        pass

    def get_name(self):
        return self.name


global_number_of_variables = 0


