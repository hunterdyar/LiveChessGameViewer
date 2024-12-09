using System;

namespace Chess
{
    [Serializable]
    public struct Info
    {
        private string id;
        public Variant variant;
        public string speed;
        public string perf;
        public bool rated;
        public string fen;
        public int turns;
        public string source;
        public Status status;
        public string winner;
        public string player;
        public string lastMove;
        public Players players;
    }

    [Serializable]
    public struct Variant
    {
        public string key;
        public string name;
        // public string short;
    }

    [Serializable]
    public struct Players
    {
        public Player white;
        public Player black;
    }

    [System.Serializable]
    public struct Player
    {
        public User user;
        public int rating;
    }

    [Serializable]
    public struct User
    {
        public string name;
        public string id;
    }

    [Serializable]
    public struct Status
    {
        public int id;
        public string name;
    }
}