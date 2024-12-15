namespace Chess
{
	public struct SquareData
	{
		public int Rank;
		public int File;
		public Piece? Piece;

		public SquareData(int rank, int file, Piece? piece)
		{
			Rank = rank;
			File = file;
			Piece = piece;
		}
	}
}