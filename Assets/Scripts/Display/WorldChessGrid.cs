using System;
using Chess;
using UnityEngine;

namespace DefaultNamespace
{
	public class WorldChessGrid : MonoBehaviour
	{
		public float gridSize;

		public Vector3 GetWorldPosition(ChessPosition position)
		{
			return GetWorldPosition(position.Rank, position.File);
		}
		public Vector3 GetWorldPosition(int rank, int file)
		{
			//todo: local to world rotation too
			return transform.position + new Vector3(rank, 0,file) * gridSize + Vector3.up * gridSize/2;
		}
		private void OnDrawGizmos()
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					Gizmos.color = Color.blue;
					Gizmos.DrawWireCube(transform.position + new Vector3(i, 0, j)*gridSize+Vector3.up*gridSize , Vector3.one * gridSize);
				}
			}
		}
	}
}