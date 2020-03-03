using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

using GlmNet;

namespace XEngine.Resources
{
	using XEngine;
	using XEngine.Shading;
	using XEngine.Common;

    public static partial class Resource
	{
		public static async Task<GeometricShape> LoadModel(string model, bool textured = true)
		{
			var positions = new List<vec3>(1024);
			var color = new vec3(0.0f, 0.0f, 0.0f);
			var normals = new List<vec3>(1024);
			var uvs = textured ? new List<vec2>(1024) : null;

			var vertices = new List<vertex>(1024);
			var indices = new List<int>(1024);
			var cache = new Dictionary<string, int>();

			using (var stream = ManifestResourceManager.LoadFromResources($"Models/{model}.obj"))
			{
				using (var reader = new StreamReader(stream))
				{
					while (!reader.EndOfStream)
					{
						var line = await reader.ReadLineAsync();

						if (string.IsNullOrWhiteSpace(line)) continue;

						if (line[0] == '#') continue;

						if (line.StartsWith("mtllib")) continue;

						if (line[0] == 'o') continue;

						if (line.StartsWith("usemtl")) continue;

						if (line[0] == 's') continue;

						switch (line[0])
						{
							case 'v':
								{
									var pieces = line.Split(' ');

									var vector = new vec3
									(
										Convert.ToSingle(pieces[1], CultureInfo.InvariantCulture),
										Convert.ToSingle(pieces[2], CultureInfo.InvariantCulture),
										pieces.Length > 3 ?
											Convert.ToSingle(pieces[3], CultureInfo.InvariantCulture)
											:
											0.0f
									);

									switch (line[1])
									{
										case ' ': positions.Add(vector); break;
										case 'n': normals.Add(vector); break;
										case 't': uvs?.Add(new vec2(vector.x, 1.0f - vector.y)); break;
										default: break;
									}
								}
								break;
							case 'f':
								{
									var pieces = line.Split(' ');

									void UseVertex(string vertex)
									{
										var data = vertex.Split('/');

										var pos = Convert.ToInt32(data[0]) - 1;
										var tex = Convert.ToInt32(data[1]) - 1;
										var nor = Convert.ToInt32(data[2]) - 1;

										var vert = new vertex(positions[pos], color, normals[nor], textured ? uvs[tex] : vector2.zero);
										var desc = vert.ToString();

										if (cache.TryGetValue(desc, out var index))
										{
											indices.Add(index);
										}
										else
										{
											index = vertices.Count;
											vertices.Add(vert);
											indices.Add(index);
											cache.Add(desc, index);
										}
									}

									UseVertex(pieces[1]);
									UseVertex(pieces[2]);
									UseVertex(pieces[3]);
								}
								break;
							default: break;
						}
					}
				}
			}

			return new GeometricShape(new ShapeData(vertices.ToArray(), indices.ToArray()));
		}
	}
}
