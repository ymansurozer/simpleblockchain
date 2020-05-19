using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SimpleBlockchain
{
    class Blockchain
    {
        public List<Node> Nodes { get; set; }
        public List<Block> Chain { get; set; }
        public List<Transaction> TransactionPool { get; set; }
        public Node GenesisNode { get; set; }
        public int Difficulty { get; set; }

        public Blockchain(int difficulty, string genesisData)
        {
            Difficulty = difficulty;
            Nodes = new List<Node>();
            Chain = new List<Block>();
            TransactionPool = new List<Transaction>();
            GenesisNode = new Node(this);

            // Create the 1st transaction
            new Transaction(this, genesisData);
           
            // Create new list to avoid referene type issues due to clearing TransactionPool afterwards
            var CurrentTransactions = new List<Transaction>();
            foreach (var t in TransactionPool)
                CurrentTransactions.Add(t);

            // Append genesis block
            AppendBlock(GenesisNode.Mine());
        }
        
        public void AppendBlock(Block block)
        {
            // Broadcast to nodes and have them validate the block
            var validation = 0;
            foreach (var node in Nodes)
                if (node.Validate(block))
                    validation++;

            // Append block with 51% consensus
            if (validation>Nodes.Count/2)
            {
                Chain.Add(block);
                TransactionPool.Clear();
            }
        }

        public string TransactionsToString(List<Transaction> transactions)
        {
            var SB = new StringBuilder();

            for (int i = 0; i < transactions.Count; i++)
                SB.Append("\n    T" + (i+1) + ": " + transactions[i].Data);

            return SB.ToString();
        }

        public string BlockToString(Block block)
        {
            var SB = new StringBuilder();
            SB.Append(block.Index.ToString() + " " + block.PoW + " " + block.PrevHash + " " + block.TimeStamp)
                .Append(TransactionsToString(block.Transactions));

            return SB.ToString();
        }
        
        public string BlockHash(Block block)
        {
            var bytes = Encoding.UTF8.GetBytes(BlockToString(block));
            var hash = new StringBuilder();

            using (var hasher = new SHA256Managed())
            {
                var result = hasher.ComputeHash(bytes);

                foreach (var b in result)
                    hash.Append(b.ToString("x2"));
            }

            return hash.ToString();
        }

        public void DisplayBlocks()
        {
            Console.WriteLine("_________________ BLOCKCHAIN DATA __________________");
            for (int i = 0; i < Chain.Count; i++)
            {
                Console.Write("> BLOCK #" + (i+1) + ": \n    HEADER: ");
                Console.WriteLine(BlockToString(Chain[i]));
                Console.WriteLine();
            }
        }

        public void DisplayQueue()
        {
            Console.WriteLine("______________ TRANSACTIONS IN QUEUE _______________");
            for (int i = 0; i < TransactionPool.Count; i++)
            {
                Console.Write("> T" + (i+1) + ": ");
                Console.WriteLine(TransactionPool[i].Data);
                Console.WriteLine();
            }
        }
    }

    class Block
    {
        public int Index { get; set; }
        public string TimeStamp { get; set; }
        public List<Transaction> Transactions { get; set; }
        public string PrevHash { get; set; }
        public int PoW { get; set; }

        public Block(int index, string timestamp, List<Transaction> transactions, string prevHash, int pow)
        {
            Index = index;
            TimeStamp = timestamp;
            Transactions = transactions;
            PrevHash = prevHash;
            PoW = pow;
        }
    }
    
    class Transaction
    {
        public string Data { get; set; }

        public Transaction(Blockchain bc, string data)
        {
            Data = data;
            bc.TransactionPool.Add(this);
        }
    }

    class Node
    {
        public Blockchain BC { get; set; }
        
        public Node(Blockchain bc)
        {
            BC = bc;
            BC.Nodes.Add(this);
        }

        public Block Mine()
        {
            var Index = BC.Chain.Count + 1;
            var TimeStamp = DateTime.Now.ToString();

            var CurrentTransactions = new List<Transaction>();
            foreach (var t in BC.TransactionPool)
                CurrentTransactions.Add(t);
            var TransactionData = BC.TransactionsToString(CurrentTransactions);

            // Checks if the block is the genesis block
            string PrevHash;
            if (BC.Chain.Count == 0)
                PrevHash = "0";
            else
                PrevHash = BC.BlockHash(BC.Chain[BC.Chain.Count - 1]);

            // Find proof of work
            var PoW = 0;
            var Block = new Block(Index, TimeStamp, CurrentTransactions, PrevHash, PoW);

            while(true)
            {
                if (Validate(Block))
                    break;
                else
                {
                    PoW++;
                    Block.PoW = PoW;
                }
            }
            
            Block.PoW = PoW;
            return Block;
        }

        public bool Validate(Block block)
        {
            var Hash = BC.BlockHash(block);

            for (int i = 1; i <= BC.Difficulty; i++)
                if (Hash[Hash.Length - i] != '0')
                    return false;

            return true;
        }
    }

    static class MenuHandler
    {
        private static bool _chainCreated = false;
        
        static void WriteHeader()
        {
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("----■ SIMPLE BLOCKCHAIN DATA STRUCTURE CREATOR ■----");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();
        }

        static void WriteMenu()
        {
            if(!_chainCreated)
                Console.WriteLine("(1) Create blockchain");

            else
            {
                Console.WriteLine("A blockchain has been created with your genesis data.\n");
                Console.WriteLine("(2) New transaction");
                Console.WriteLine("(3) View pending transactions");
                Console.WriteLine("(4) Mine block");
                Console.WriteLine("(5) Display blockchain data");
            }
            Console.WriteLine("(Other) Exit");
            Console.WriteLine();
        }

        static string GetInstruction(string instruction)
        {
            Console.WriteLine(instruction + ":");
            Console.Write("> ");
            return Console.ReadLine().ToLower();
        }

        static void Wait()
        {
            Console.WriteLine("\nPress any key to go back.");
            Console.ReadKey();
        }
        
        public static void Menu()
        {
            string Input;
            int Difficulty;
            string GenesisData;
            Blockchain BC = new Blockchain(1,"1");

            while (true)
            {
                Console.Clear();
                WriteHeader();
                WriteMenu();
                Input = GetInstruction("Your instruction");

                Console.Clear();
                WriteHeader();
                switch (Input)
                {
                    case "1":
                        if (_chainCreated)
                        {
                            Console.WriteLine("You have already created a blockchain!");
                            Wait();
                        }
                        else
                        {
                            Console.WriteLine("Specify difficulty (number of zeroes required at the end of the hash).");
                            Difficulty = Convert.ToInt32(GetInstruction("Write a number"));

                            Console.WriteLine("\nProvide genesis data (first data to be written to the first block)");
                            GenesisData = GetInstruction("Write anything");

                            Console.WriteLine("\nMining first block...");

                            BC = new Blockchain(Difficulty, GenesisData);
                            Console.WriteLine($"\nBlockchain with difficulty ({Difficulty}) successfully created!");
                            _chainCreated = true;

                            Wait();
                        }
                        continue;
                    case "2":
                        new Transaction(BC, GetInstruction("Provide the data for this transaction"));
                        Console.WriteLine("\nTransaction successfully created!");
                        Wait();
                        continue;
                    case "3":
                        BC.DisplayQueue();
                        Wait();
                        continue;
                    case "4":
                        Console.WriteLine("Transactions grouped into a block. Mining...");

                        BC.AppendBlock(BC.GenesisNode.Mine());
                        Console.WriteLine("\nMining complete! Block successfully appended to the chain!");
                        Wait();
                        continue;
                    case "5":
                        BC.DisplayBlocks();
                        Wait();
                        continue;
                    default:
                        break;
                }
                Console.WriteLine("Thank you!");
                Console.ReadKey();
                break;
            }
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            MenuHandler.Menu();
        }
    }
}
