using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkMeshData{
  public bool isMainMesh;

  private readonly List<Vector3> vertices = new();
  private readonly List<int> triangles = new();
  private readonly List<Vector3> normals = new();
  private readonly List<Vector2> uvs = new();
  private readonly List<Color32> colors = new();
  
  public ChunkMeshData transparentMeshData;
  public ChunkMeshData noCullMeshData;

  public ChunkMeshData(bool isMainMesh = true){
    this.isMainMesh = isMainMesh;
    
    if (isMainMesh){
      transparentMeshData = new ChunkMeshData(false);
      noCullMeshData = new ChunkMeshData(false);
    }
  }
  
  public void AddTextureFace(TextureMapper.TextureMap.Face face){
    uvs.Add(face.bl);
    uvs.Add(face.tl);
    uvs.Add(face.tr);
    uvs.Add(face.br);
  }

  public void AddColors(TextureMapper.TextureMap textureMap, byte lBL, byte lTL, byte lTR, byte lBR){
    Color32 c = textureMap.defaultColor;
    colors.Add(new Color32(c.r, c.g, c.b, lBL));
    colors.Add(new Color32(c.r, c.g, c.b, lTL));
    colors.Add(new Color32(c.r, c.g, c.b, lTR));
    colors.Add(new Color32(c.r, c.g, c.b, lBR));
  }

  public void AddFace(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 normal){
    int index = vertices.Count;
    vertices.Add(a);
    vertices.Add(b);
    vertices.Add(c);
    vertices.Add(d);
    normals.Add(normal);
    normals.Add(normal);
    normals.Add(normal);
    normals.Add(normal);
    triangles.Add(index + 0);
    triangles.Add(index + 1);
    triangles.Add(index + 2);
    triangles.Add(index + 2);
    triangles.Add(index + 3);
    triangles.Add(index + 0);
  }

  public void SetupMesh(Mesh mesh){
    mesh.subMeshCount = 3;

    // Combine vertices, uvs, normals, colors
    mesh.SetVertices(vertices.Concat(transparentMeshData.vertices).Concat(noCullMeshData.vertices).ToArray());

    // Set triangles for each sub-mesh
    mesh.SetTriangles(triangles, 0); // Opaque mesh
    mesh.SetTriangles(transparentMeshData.triangles.Select(val => val + vertices.Count).ToArray(), 1); // Transparent mesh
    mesh.SetTriangles(noCullMeshData.triangles.Select(val => val + vertices.Count + transparentMeshData.vertices.Count).ToArray(), 2); // No Cull mesh

    // Combine UVs, Normals, and Colors for all meshes
    mesh.SetUVs(0, uvs.Concat(transparentMeshData.uvs).Concat(noCullMeshData.uvs).ToArray());
    mesh.SetNormals(normals.Concat(transparentMeshData.normals).Concat(noCullMeshData.normals).ToArray());
    mesh.SetColors(colors.Concat(transparentMeshData.colors).Concat(noCullMeshData.colors).ToArray());

    // Clear all the mesh data
    vertices.Clear();
    triangles.Clear();
    colors.Clear();
    uvs.Clear();
    normals.Clear();

    // Clear transparent mesh data
    transparentMeshData.vertices.Clear();
    transparentMeshData.triangles.Clear();
    transparentMeshData.colors.Clear();
    transparentMeshData.uvs.Clear();
    transparentMeshData.normals.Clear();

    // Clear the no cull mesh data
    noCullMeshData.vertices.Clear();
    noCullMeshData.triangles.Clear();
    noCullMeshData.colors.Clear();
    noCullMeshData.uvs.Clear();
    noCullMeshData.normals.Clear();
  }
}