using System;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chess
{
	public struct ChessPosition : IEquatable<ChessPosition>
	{
		private static char[] _files = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

		public int Rank;
		public int File;
		
		public ChessPosition(int rank, int file)
		{
			Rank = rank;
			File = file;
			Validate();
		}

		public ChessPosition(string rf)
		{
			File = Array.IndexOf(_files, rf[0]);
			Rank = int.Parse(rf[0].ToString())-1;
			Validate();
		}

		public ChessPosition(char file, char rank)
		{
			File = Array.IndexOf(_files, file);
			if(!int.TryParse(rank.ToString(), out Rank))
			{
				Debug.LogError($"Can't Parse rank {rank}. File is {file}");
			}
			else
			{
				Rank -= 1;
			}
			Validate();
		}

		public ChessPosition(int rank, char file)
		{
			File = Array.IndexOf(_files, file);
			Rank = rank;
		}

		private void Validate()
		{
			if (File < 0 || File >= 8 || Rank < 0 || Rank > 8)
			{
				throw new InvalidDataException("Invalid Position");
			}
		}

		// public bool Equals()
		// {
		// 	return Rank == Rank && File == File;
		// }

		public override string ToString()
		{
			//0,0 = a1
			return _files[File].ToString()+File;
		}

		public static string XYToRankFile(int x, int y)
		{
			return $"{x + 1}{_files[y]}";
		}

		public bool Equals(ChessPosition other)
		{
			return Rank == other.Rank && File == other.File;
		}

		public override bool Equals(object obj)
		{
			return obj is ChessPosition other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Rank, File);
		}

		public static bool operator ==(ChessPosition left, ChessPosition right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ChessPosition left, ChessPosition right)
		{
			return !left.Equals(right);
		}
	}
}