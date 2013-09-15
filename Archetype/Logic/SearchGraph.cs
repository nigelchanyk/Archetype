using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using Mogre;

using Archetype.Objects;
using Archetype.Utilities;

namespace Archetype.Logic
{
	public class SearchGraph
	{
		private LinkedList<SourceVertex> Vertices = new LinkedList<SourceVertex>();
		private World World;

		public SearchGraph(World world)
		{
			this.World = world;
		}

		public void AddVertex(Vector3 position)
		{
			SourceVertex newVertex = new SourceVertex(position, Vertices.Count);

			//add edges between vertices if connectable
			foreach (SourceVertex vertex in Vertices)
			{
				if (World.IsValidPath(vertex.Position, position))
				{
					float distance = vertex.Position.Distance(position);

					vertex.Edges.AddLast(new Edge(newVertex, distance));
					newVertex.Edges.AddLast(new Edge(vertex, distance));
				}
			}

			Vertices.AddLast(newVertex);
		}

		public Vector3[] GetAdjacentVertices(Vector3 source)
		{
			SourceVertex vertex = Vertices.FirstOrDefault(x => x.Position.IsApproximately(source, 0.0009f));
			if (vertex == null)
				return new Vector3[0];
			return vertex.Edges.Select(x => x.Destination.Position).ToArray();
		}

		public Vector3? GetClosestVertex(Vector3 source)
		{
			float minDistance = float.MaxValue;
			Vector3? dest = null;

			foreach (SourceVertex vertex in Vertices)
			{
				if (World.IsValidPath(source, vertex.Position))
				{
					float currentDistance = source.SquaredDistance(vertex.Position);
					if (currentDistance < minDistance)
					{
						minDistance = currentDistance;
						dest = vertex.Position;
					}
				}
			}

			return dest;
		}

		public Path Search(Vector3 source, Vector3 destination)
		{
			//temporary add source and destination as vertices
			AddVertex(source);
			AddVertex(destination);

			int sourceIndex = Vertices.Count - 2;
			int destIndex = Vertices.Count - 1;
			float[] dist = new float[Vertices.Count];
			SourceVertex[] previous = new SourceVertex[Vertices.Count];
			SourceVertex[] verticesArray = Vertices.ToArray();
			MinHeap heap = new MinHeap(verticesArray.Length);
			LinkedList<Vector3> result = new LinkedList<Vector3>();

			//Dijkstra's algorithm

			//initialize everything to maximum distance
			for (int i = 0; i < dist.Length; i++)
				dist[i] = float.MaxValue;

			//set source distance to 0
			dist[sourceIndex] = 0;

			//add all vertices to heap
			for (int i = 0; i < dist.Length; i++)
				heap.Add(dist[i], i);

			while (heap.Count > 0)
			{
				int u = heap.Poll().Value;

				if (u == destIndex)
				{
					//destination reached
					result.AddFirst(verticesArray[u].Position);
					while (previous[u] != null)
					{
						//backtrack to source
						result.AddFirst(previous[u].Position);
						u = previous[u].Index;
					}
					break;
				}

				//no valid paths
				if (dist[u] == float.MaxValue)
					break;

				//iterate through all edges
				foreach (Edge edge in verticesArray[u].Edges)
				{
					int v = edge.Destination.Index;

					//only check v if v is still in the heap
					if (!heap.ContainsValue(v))
						continue;

					//calculate distance of current path
					float alt = dist[u] + edge.Distance;
					if (alt < dist[v])
					{
						//current path is shorter
						dist[v] = alt;
						previous[v] = verticesArray[u];
						heap.Update(alt, v);
					}
				}
			}

			//remove source and destination from vertices list
			RemoveLastVertex();
			RemoveLastVertex();

			if (result.Count == 0)
				return null;

			return new Path(result.ToArray());
		}

		private void RemoveLastVertex()
		{
			SourceVertex last = Vertices.Last.Value;
			Vertices.RemoveLast();

			foreach (SourceVertex vertex in Vertices)
			{
				if (vertex.Edges.Last != null && vertex.Edges.Last.Value.Destination == last)
					vertex.Edges.RemoveLast();
			}
		}


		private class SourceVertex
		{
			public Vector3 Position { get; private set; }
			public LinkedList<Edge> Edges { get; private set; }
			public int Index { get; private set; }

			public SourceVertex(Vector3 position, int index)
			{
				this.Position = position;
				this.Edges = new LinkedList<Edge>();
				this.Index = index;
			}
		}


		private class Edge
		{
			public SourceVertex Destination { get; private set; }
			public float Distance { get; private set; }

			public Edge(SourceVertex dest, float dist)
			{
				this.Destination = dest;
				this.Distance = dist;
			}
		}
	}
}
