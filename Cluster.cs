using System;
using System.Collections.Generic;
using System.Linq;

namespace clopeAlg
{
    public class Cluster
    {
        private int _numberTransactions;
        private int _square;
        private double _repulsion;
        private Dictionary<string, int> _occ = new Dictionary<string, int>();

        public double Repulsion
        {
            get => _repulsion;
            set
            {
                _repulsion = value;
            }
        }

        public int NumberTransactions
        {
            get => _numberTransactions;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(NumberTransactions), "NumberTransactions < 0");
                }
                _numberTransactions = value;
            }
        }
        public int Width
        {
            get => _occ.Count();
        }
        public int Square
        {
            get => _square;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Square), "Square < 0");
                }

                _square = value;
            }
        }
        public Dictionary<string, int> OCC
        {
            get => _occ;
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="rep"> коэффициент отталкивания</param>
        public Cluster(double repulsion)
        {
            _numberTransactions = 0;
            _repulsion = repulsion;
        }

        /// <summary>
        /// Добавление транзакции в кластер
        /// </summary>
        /// <param name="transaction">транзакция</param>
        public void Add(IEnumerable<string> transaction)
        {
            _square += transaction.Count();
            _numberTransactions += 1;
            foreach (var tranElement in transaction)
            {
                if (!_occ.ContainsKey(tranElement))
                {
                    _occ.Add(tranElement, 0);/////////////
                }
                _occ[tranElement] += 1;
            }
        }

        /// <summary>
        /// Удаление транзакции из кластера
        /// </summary>
        /// <param name="transaction">транзакция</param>
        public void Delete(IEnumerable<string> transaction)
        {
            _square -= transaction.Count();
            _numberTransactions -= 1;
            foreach (var tranElement in transaction)
            {
                _occ[tranElement] -= 1;
                //if (!IsElementInCluster(tranElement))/////////////
                //    _width--;
                if (_occ[tranElement] == 0)
                {
                    _occ.Remove(tranElement);
                }
            }
        }

        /// <summary>
        /// Вычисление прироста при добавлении транзакции
        /// </summary>
        /// <param name="transaction">транзакция</param>
        /// <returns>величина прироста</returns>
        public double DeltaAdd(IEnumerable<string> transaction)
        {
            var S_new = _square + transaction.Count();
            var W_new = Width;
            var N_new = _numberTransactions + 1;

            if (_numberTransactions == 0)
            {
                W_new = transaction.Distinct().Count();/////
                return S_new * N_new / Math.Pow(W_new, _repulsion);
            }

            foreach (var tranElement in transaction)
            {
                if (!_occ.ContainsKey(tranElement))
                {
                    W_new++;
                }
            }

            return (S_new * N_new / Math.Pow(W_new, _repulsion))
                - (_square * _numberTransactions / Math.Pow(Width, _repulsion));
        }

        /// <summary>
        /// Вычисление прироста при удалении транзакции
        /// </summary>
        /// <param name="transaction">транзакция</param>
        /// <returns>величина прироста</returns>
        public double DeltaSub(IEnumerable<string> transaction)
        {
            var S_new = _square - transaction.Count();
            var W_new = Width;
            var N_new = _numberTransactions - 1;

            if (_numberTransactions == 1)
            {
                return -_square * _numberTransactions / Math.Pow(Width, _repulsion);
            }

            foreach (var tranElement in transaction)
            {
                if (_occ[tranElement] == 1)
                {
                    W_new--;
                }
            }

            return S_new * N_new / Math.Pow(W_new, _repulsion)
                - _square * _numberTransactions / Math.Pow(Width, _repulsion);
        }
    }
}
