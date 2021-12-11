using System.Collections.Generic;
using System.IO;
using System.Linq;
using SearchWithVariables.Model;
using static SearchWithVariables.Helpers.RegexpHelper;

namespace SearchWithVariables.Logic
{
    /// <summary>
    /// Алгоритм поиска с переменными от цели (в глубину).
    /// </summary>
    /// <returns></returns>
    class Searcher
    {
        public List<Vertex> Facts { get; } // список фактов (предикатов)
        public List<Rule> Rules { get; } // список правил
        public Vertex Target { get; } // цель

        public Stack<Vertex> OpenedVertices; // список открытых вершин
        public Stack<Rule> OpenedRules; // список открытых правил
        public List<Rule> ClosedRules; // список закрытых правил

        /// <summary>
        /// Список подстановок. Представляет собой стек подстановок,
        /// так как хранит подстановки на каждом шаге алгоритма.
        /// Подстановки представлены словарём пар ключ-значение,
        /// в котором ключ - аргумент, который будет заменён,
        /// значение - новый аргумент, на который будет произведена замена.
        /// </summary>
        public Stack<Dictionary<Argument, Argument>> Substitutions { get; }

        private int _iteration;
        private Vertex _currentTarget;
        private Item _currentDescendent;
        public bool DebugMode { get; set; }
        public string DebugLog { get; private set; }

        public Searcher(List<Vertex> facts, List<Rule> rules,
            Vertex target, bool debugMode = false)
        {
            Facts = facts;
            Rules = rules;
            Target = target;

            OpenedVertices = new Stack<Vertex>();
            OpenedRules = new Stack<Rule>();
            ClosedRules = new List<Rule>();
            Substitutions = new Stack<Dictionary<Argument, Argument>>();

            // В список открытых вершин добавляем цель.
            OpenedVertices.Push(Target);

            _iteration = 0;
            _currentDescendent = null;
            _currentTarget = null;
            DebugMode = debugMode;
            DebugLog = "";
        }

        private void UpdateDebugLog()
        {
            if (DebugMode)
            {
                DebugLog += GetStepData();
            }
        }

        /// <summary>
        /// Проверка, являются ли предикаты унифицируемыми.
        /// </summary>
        /// <param name="target">Целевой предикат</param>
        /// <param name="sample">Предикат-образец</param>
        /// <returns>true, если предикаты унифицируемы, false - иначе</returns>
        private bool CheckUnification(Vertex target, Vertex sample)
        {
            // Проверяем, что имена и количество аргументов совпадают.
            if (target.Label != sample.Label) return false;
            if (target.Arguments.Count != sample.Arguments.Count) return false;

            for (var i = 0; i < target.Arguments.Count; i++)
            {
                // Если в одной и той же позиции в списке аргументов
                // стоят константы, то они должны быть равны.
                if (sample.Arguments[i].IsConst() && target.Arguments[i].IsConst())
                {
                    if (!target.Arguments[i].Equals(sample.Arguments[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Процедура унификации. Выполняет замену в целевом предикате так,
        /// чтобы он совпадал с предикатом-образцом, и сохраняет список прозведённых подстановок.
        /// </summary>
        /// <param name="target">Целевой предикат.</param>
        /// <param name="sample">Предикат-образец.</param>
        /// <returns>Целевой предикат, в котор</returns>
        private Vertex Unify(Vertex target, Vertex sample)
        {
            var unifiedArguments = new Argument[target.Arguments.Count];
            var substitutions = new Dictionary<Argument, Argument>();
            for (var i = 0; i < target.Arguments.Count; i++)
            {
                // Если аргумент целевого предиката - переменная,
                // то подставляем в неё константу или переменную из предиката-образца.
                if (!target.Arguments[i].IsConst())
                {
                    unifiedArguments[i] = sample.Arguments[i];
                    substitutions.Add(target.Arguments[i], sample.Arguments[i]);
                }
                else // иначе оставляем аргумент целевого предиката
                {
                    unifiedArguments[i] = target.Arguments[i];
                }
            }

            Substitutions.Push(substitutions);

            return new Vertex(target.Label, unifiedArguments);
        }
        
        /// <summary>
        /// Выполняет подстановки в предикате с учётом списка сохранённых подстановок.
        /// </summary>
        /// <param name="predicate">Предикат, в котором нужно выполнить замену.</param>
        /// <returns>Новый предикат с выполненными подстановками.</returns>
        public Vertex PerformSubstitution(Vertex predicate)
        {
            // Извлекаем список подстановок с предыщего шага алгоритма, если он был.
            var substitutions = Substitutions.Count > 0
                ? Substitutions.Peek()
                : new Dictionary<Argument, Argument>();
            
            var newArguments = new Argument[predicate.Arguments.Count];
            for (var j = 0; j < predicate.Arguments.Count; j++)
            {
                // Если в списке подстановок есть замена для текущего аргумента -
                // заменяем, иначе - оставляем текущий аргумент.
                var argument = predicate.Arguments[j];
                newArguments[j] = substitutions.Keys.Contains(argument)
                    ? substitutions[argument]
                    : argument;
            }

            return new Vertex(predicate.Label, newArguments);
        }

        // Подстановки для списка вершин.
        public List<Vertex> PerformSubstitution(List<Vertex> predicates)
        {
            return predicates.Select(PerformSubstitution).ToList();
        }

        // Подстановки для стека вершин.
        private Stack<Vertex> PerformSubstitution(Stack<Vertex> predicates)
        {
            var temp = PerformSubstitution(predicates.ToList());
            temp.Reverse();
            return new Stack<Vertex>(temp);
        }

        // Подстановки для стека правил.
        private Stack<Rule> PerformSubstitution(Stack<Rule> rules)
        {
            var updatedRules = new List<Rule>();
            foreach (var rule in rules)
            {
                // Унифицируем выходную вершину правила.
                var unifiedResult = PerformSubstitution(rule.Result);
                // Унифицируем список предикатов правила.
                var unifiedPredicates = PerformSubstitution(rule.Predicates);
                // Получаем новое правило с подстановками.
                updatedRules.Add(new Rule(rule.Label, unifiedPredicates, unifiedResult));
            }

            updatedRules.Reverse();
            return new Stack<Rule>(updatedRules);
        }

        /// <summary>
        /// Проверяет покрытие предикатов (конъюнктов) правила фактами.
        /// </summary>
        /// <param name="rule">Правило для проверки.</param>
        /// <returns>Возвращает список недоказанных предикатов.</returns>
        private List<Vertex> ProveWithFacts(Rule rule)
        {
            var unprovenPredicates = new List<Vertex>();
            foreach (var predicate in rule.Predicates)
            {
                var predicateProven = false;
                // Сравнивает предикат правила с фактами.
                for (var i = 0; i < Facts.Count && !predicateProven; i++)
                {
                    if (predicate.Equals(Facts[i]))
                    {
                        predicateProven = true;
                    }
                }

                if (!predicateProven)
                {
                    unprovenPredicates.Add(predicate);
                }
            }

            return unprovenPredicates;
        }

        /// <summary>
        /// Доказывает правило и сравнивает его выходную вершину с целью.
        /// </summary>
        /// <param name="rule">Правило, в котором все конънкты доказаны (покрыты фактами).</param>
        /// <returns>true, если выходная вершина совпадает с целью (решение найдено),
        /// false - иначе.</returns>
        private bool ProveAndCompareWithTarget(Rule rule)
        {
            // Добавляем выходную вершину правила в список фактов.
            Facts.Add(rule.Result);

            // Удаляем выходную вершину правила из списка открытых вершин.
            if (OpenedVertices.Contains(rule.Result))
            {
                var temp = OpenedVertices.ToList();
                temp.Remove(rule.Result);
                temp.Reverse();
                OpenedVertices = new Stack<Vertex>(temp);
            }

            // Добавляем правило в список закрытых.
            ClosedRules.Add(Rules.Find(r => r.Label == rule.Label));

            // Сравниваем выходную вершину правила с целью.
            return rule.Result.Equals(Target);
        }

        /// <summary>
        /// Метод поиска потомка. Находит вершину (может являться фактом или
        /// выходной вершиной правила), унифируемую с заданной вершиной-подцелью.
        /// </summary>
        /// <param name="target">Вершина-подцель.</param>
        /// <returns>Вершина, уницируемая с подцелью, или правило, выходная вершина которого
        /// унифицируема с подцелью, или null, если ничего не найдено.</returns>
        private Item FindDescendant(Vertex target)
        {
            // Просматриваем базу фактов, проверяя его на унификацию с подцелью.
            foreach (var fact in Facts)
            {
                if (CheckUnification(target, fact))
                {
                    return fact;
                }
            }

            // Просматриваем список правил, проверяя его выходную вершину на унификацию с подцелью.
            foreach (var rule in Rules)
            {
                if (rule.State == Item.States.CLOSED) continue;

                if (CheckUnification(target, rule.Result))
                {
                    rule.SetState(Item.States.CLOSED);
                    return rule;
                }
            }

            return null;
        }

        /// <summary>
        /// Метод разметки вершин.
        /// </summary>
        /// <param name="rule">Правило, в котором все конънкты доказаны (покрыты фактами).</param>
        /// <returns>true, если выходная вершина совпадает с целью (решение найдено),
        /// false - иначе.</returns>
        private bool Markup(Rule rule)
        {
            // Распространяем переменные из подстановок в список открытых вершин.
            OpenedVertices = PerformSubstitution(OpenedVertices);

            // Добавляем правило в список доказанных, сверяя его выходную вершину с целью.
            if (ProveAndCompareWithTarget(rule)) return true;

            // Проверяем предыдущее правило на покрытие фактами.
            if (OpenedRules.Count > 0)
            {
                var openedRule = OpenedRules.Peek();
                var unprovenPredicates = ProveWithFacts(openedRule);
                if (unprovenPredicates.Count == 0)
                {
                    OpenedRules.Pop();
                    if (ProveAndCompareWithTarget(openedRule)) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Поиск с переменными от цели (в глубину).
        /// </summary>
        /// <returns>Найдено ли решение.</returns>
        public bool Search()
        {
            bool solved = false;

            do
            {
                // Получаем верхнюю вершину из стека открытых вершин.
                var head = _currentTarget = OpenedVertices.Peek();
                // Находим потомка - может быть как фактом, так и правилом (выходной вершиной правила)
                var descendant = FindDescendant(head);

                if (descendant is Vertex fact) // если нашли вершину (подцель унифицируема с фактом)
                {
                    OpenedVertices.Pop(); // убираем верхнюю вершину из списка открытых
                    // Выполняем унификацию, находя и сохраняя новую подстановку.
                    Unify(head, fact);
                    // Распространяем переменные из новой подстановки на открытые вершины и открытые правила.
                    OpenedVertices = PerformSubstitution(OpenedVertices);
                    OpenedRules = PerformSubstitution(OpenedRules);

                    // Проверяем, доказано ли предыдущее открытое правило.
                    var rule = OpenedRules.Peek();
                    var unprovenPredicates = ProveWithFacts(rule);
                    if (unprovenPredicates.Count == 0) // все предикаты правила доказаны
                    {
                        OpenedRules.Pop(); // удаляем правило из списка открытых
                        // Вызываем метод разметки вершин, проверяя, найдено ли решение.
                        solved = Markup(rule);
                    }
                }
                else if (descendant is Rule rule) // если нашли правило (подцель унифицируема с результатом правила)
                {
                    // Унифицируем результат правила и его предикаты.
                    var unifiedResult = Unify(rule.Result, head);
                    var unifiedPredicates = PerformSubstitution(rule.Predicates);

                    // Получаем новое правило с подстановками и сохраняем его в списке открытых правил.
                    var updatedRule = new Rule(rule.Label, unifiedPredicates, unifiedResult);
                    OpenedRules.Push(updatedRule);

                    // Проверяем, есть ли у правила недоказанные предикаты.
                    var unprovenPredicates = ProveWithFacts(updatedRule);
                    if (unprovenPredicates.Count > 0) // правило не доказано
                    {
                        // Добавляем недоказанные вершины в список открытых вершин.
                        unprovenPredicates.Reverse();
                        unprovenPredicates.ForEach(predicate =>
                            OpenedVertices.Push(predicate));
                    }
                    else // все предикаты правила доказаны
                    {
                        OpenedRules.Pop(); // удаляем правило из списка открытых
                        // Вызываем метод разметки вершин, проверяя, найдено ли решение.
                        solved = Markup(updatedRule);
                    }
                }
                else // не нашли потомка
                {
                    OpenedVertices.Pop(); // убираем верхнюю вершину из списка открытых
                }

                _iteration++;
                _currentDescendent = descendant;
                UpdateDebugLog();
            } while (OpenedVertices.Count > 1 && !solved);

            return solved;
        }

        public string GetDataAsString()
        {
            var result = "";
            result += "Факты:\n";
            result += string.Join(", ", Facts);

            result += "\n\nПравила: \n";
            result += string.Join("\n", Rules);

            result += $"\n\nЦель: {Target}\n\n";
            return result;
        }

        public string GetStepData()
        {
            var data = "";
            string separator = new string('-', 80);
            if (_iteration == 0)
            {
                data += "Дано:\n";
            }
            else
            {
                data += $"Шаг {_iteration}\n";
                data += separator;
                data += $"\nПодцель: {_currentTarget}";
                data += "\nПотомок: ";
                data += _currentDescendent != null
                    ? _currentDescendent.ToString()
                    : "–";
                data += "\nПодстановки: " + string.Join(", ",
                                Substitutions.Peek().Select(s =>
                                    $"{s.Key} -> {s.Value}")) + "\n";

            }
            data += separator;
            data += "\nОткрытые вершины: ";
            data += OpenedVertices.Count > 0
                ? string.Join(", ", OpenedVertices)
                : "–";
            data += "\nОткрытые правила: ";
            data += OpenedRules.Count > 0
                ? string.Join(", ", OpenedRules.Select(rule => rule.Label))
                : "–";
            data += "\nФакты: " + string.Join(", ", Facts);
            data += "\nЗакрытые правила: ";
            data += ClosedRules.Count > 0
                ? string.Join(", ", ClosedRules.Select(rule => rule.Label))
                : "–";
            data += "\n";
            data += separator;
            data += "\n";

            return data;
        }

        public static Searcher InitFromFile(string fileName)
        {
            var lines = File.ReadAllLines(fileName);

            var facts = new List<Vertex>();
            facts.AddRange(MatchPredicateList(lines[0]));
            
            var target = MatchPredicate(lines[1]);

            var rules = new List<Rule>();
            for (int i = 2; i < lines.Length; i++)
            {
                rules.Add(MatchRule(lines[i]));
            }

            return new Searcher(facts, rules, target);
        }
    }
}
