# 📘 LPDM Framework

![LPDM Framework Overview](lpdm_framework_figure.png)

A Linear Partitioning Diversity Metric for Evaluation of Permutation-based Metaheuristic Algorithms

### ✍️ Authors
- **Majid Shahbazi**
- **Faramarz Safi Esfahani**
- **Hadi Shahbazi**
- **Seyedali Mirjalili**

---

The **LPDM Framework** (Learning Phase Distribution Modeling) provides a hybrid experimentation environment designed to evaluate the performance of permutation-based metaheuristic algorithms using a novel diversity metric. This project supports both Python and C# implementations and includes empirical results, benchmark comparisons, and full methodology used in the related research paper:

📄 _"A Linear Partitioning Diversity Metric for Evaluation of Permutation-based Metaheuristic Algorithms"_ (Shahbazi et al., 2025)

## 📁 Repository Structure

```
LPDM_Framework/
│
├── Codes-Python/           # Python implementations of LPDM and benchmarks
├── Codes-C#/               # C# implementations for LPDM simulation and experiments
├── Experiment Results/     # Collected metrics and diversity analysis (CSV, XLSX)
├── README.md               # Project overview and documentation
├── LICENSE                 # MIT License
└── CITATION.cff            # Citation information for this framework
```

## 🚀 Key Features

- **Dual-language support**: Implementations in Python and C#
- **Diversity-aware evaluation**: Incorporates the LPDM metric for understanding solution diversity in permutation-based algorithms
- **Comprehensive experimental design**: Includes 18 experiment configurations across multiple strategies, operators, and benchmark functions
- **Benchmark Integration**: Ready to integrate with standard metaheuristics (e.g., GA, PSO, SA, TS, ACO, etc.)
- **Extendable and modular**: Easy to add new algorithms or diversity measures
- **Statistical Output**: CSV/XLSX reports and visualizations for comparative analysis

## 📐 Methodology Overview

The LPDM Framework assesses diversity in metaheuristic search processes using a linear partitioning model over the solution space. The approach is broken into the following phases:

1. **Search Space Encoding**  
   Permutation-based solutions are encoded and grouped via partitioned diversity spaces.

2. **Diversity Metric Computation**  
   A linear diversity score is computed by mapping each solution to predefined partitions based on linear position indices.

3. **Experimentation and Comparison**  
   The framework runs controlled experiments across standard metaheuristics with varied configurations and captures convergence behavior and diversity over time.

4. **Evaluation**  
   Performance is analyzed using metrics such as solution quality, LPDM diversity, and statistical spread across runs.

For full methodological details, please refer to the accompanying paper.

## 📊 Experimental Results

The `Experiment Results/` folder contains the outcomes of 18 structured experiments:

- Variations of selection strategies and neighborhood operators
- Impact of LPDM on convergence dynamics
- Benchmarks: Job Shop Scheduling Problems, Traveling Salesman Problems
- Results are provided in `.csv` and `.xlsx` formats with summaries of accuracy, diversity, and runtime.

## 🛠 Requirements (Python)

To run the Python-based modules:

```bash
pip install -r requirements.txt
```

## 📜 License

This project is licensed under the [MIT License](LICENSE).

## 🔖 Citation

If you use this framework in your research, please cite the following:

```bibtex
@article{shahbazi2025lpdm,
  title={A Linear Partitioning Diversity Metric for Evaluation of Permutation-based Metaheuristic Algorithms},
  author={Shahbazi, Seyed Hadi and Safi Esfahani, Faramarz and Shah-Hosseini, Hadi and Taghipour, Majid},
  journal={Journal Name},
  year={2025}
}
```

You can also refer to the citation file: `CITATION.cff`

## 🤝 Contribution

Contributions and extensions are welcome!  
To report issues or propose enhancements, feel free to open an issue or fork the repository.

---

🧪 _Explore the diversity, benchmark your algorithms, and improve your optimization strategies using LPDM!_
