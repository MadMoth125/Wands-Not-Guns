using System;
using UnityEngine;

namespace Enemy.Jobs
{
	[Serializable]
	public struct PlayerData
	{
		public PlayerData(int id, Vector3 position)
		{
			this._id = id;
			this._position = position;
		}

		public int ID => _id;
		public Vector3 Position => _position;

		private int _id;
		private Vector3 _position;

		public void SetData(int id, Vector3 position)
		{
			this._id = id;
			this._position = position;
		}
	}
}