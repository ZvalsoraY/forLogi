using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace clopeAlg
{
    internal class Program
    {
        public static double repulsion = 2.6;
        public static string path = @"C:\Users\Master\Documents\C_sharp\Repositories\forLoginom\clopeAlg\agaricus-lepiota.data";

        static void Main()
        {
            List<List<string>> transactionsList = new List<List<string>>();
            foreach (var line in ReadIenumData(path))
            {
                //string[] lineString ;
                List<string> lineString = new List<string>();
                for (int i = 1; i < line.Count(); i++)
                {
                    if (line.ElementAt(i) != "?")
                    {
                        lineString.Add(line.ElementAt(i) + i.ToString());
                    }
                }
                transactionsList.Add(lineString);
            }

            List<Cluster> clusters = new List<Cluster>();
            Dictionary<int, int> tranClusterDict = new Dictionary<int, int>();// ключ - номер транзакции, значение - номер кластера
            int selectClusterNum = 0;
            clusters.Add(new Cluster(repulsion));// добавление пустого кластера
            Cluster selectCluster = clusters[0];

            foreach (var currentTransaction in transactionsList)//инициализация
            {
                double maxProfit = 0;
                foreach (var currentCluster in clusters)
                {
                    if (currentCluster.DeltaAdd(currentTransaction) > maxProfit)
                    {
                        maxProfit = currentCluster.DeltaAdd(currentTransaction);
                        selectCluster = currentCluster;
                        selectClusterNum = clusters.IndexOf(currentCluster);
                    }
                }
                if (selectCluster.NumberTransactions == 0)
                {
                    clusters.Add(new Cluster(repulsion));
                }
                selectCluster.Add(currentTransaction);
                tranClusterDict.Add(transactionsList.IndexOf(currentTransaction), selectClusterNum);//
            }

            bool moved;
            do//Итерация
            {
                moved = false;
                for (int i = 0; i < tranClusterDict.Count; i++)
                {
                    double maxProfit = 0;
                    var currentCluster = clusters[tranClusterDict[i]];
                    var currentTransaction = transactionsList[i];
                    var delTranFromClasterCost = currentCluster.DeltaSub(currentTransaction);
                    foreach (var cluster in clusters)
                    {
                        if (cluster != currentCluster)
                        {
                            if ((cluster.DeltaAdd(currentTransaction) + delTranFromClasterCost) > maxProfit)
                            {
                                maxProfit = cluster.DeltaAdd(currentTransaction) + delTranFromClasterCost;
                                selectCluster = cluster;
                                selectClusterNum = clusters.IndexOf(cluster);
                            }
                        }
                    }
                    if (maxProfit > 0)
                    {
                        if (selectCluster.NumberTransactions == 0)
                        {
                            clusters.Add(new Cluster(repulsion));
                        }
                        selectCluster.Add(currentTransaction);
                        tranClusterDict[i] = selectClusterNum;
                        currentCluster.Delete(currentTransaction);
                        moved = true;
                    }
                }
            }
            while (moved == true);
            Console.WriteLine("{0,10}   |{1,10}   |{2,10}   |", "CLUSTER", "e", "p");
            var eCountSum = 0;
            var pCountSum = 0;
            for (int i = 0; i < clusters.Count; i++)
            {
                if (clusters[i].NumberTransactions != 0)
                {
                    var eCount = 0;
                    var pCount = 0;
                    foreach (var tranCluster in tranClusterDict.Where(x => x.Value == i))
                    {
                        if (ReadIenumData(path).ElementAt(tranCluster.Key).ElementAt(0) == "e")
                        {
                            eCount++;
                        }
                        else
                        {
                            pCount++;
                        }
                    }
                    Console.WriteLine("{0,10}   |{1,10}   |{2,10}   |", i, eCount, pCount);
                    eCountSum += eCount;
                    pCountSum += pCount;
                }
            }
            Console.WriteLine("{0,10}   |{1,10}   |{2,10}   |", "Sum", eCountSum, pCountSum);
        }
        public static IEnumerable<IEnumerable<string>> ReadIenumData(string filePath)
        {
            var lines = File.ReadLines(filePath);
            return lines.Select(line =>
            line.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s));
        }
    }
}
