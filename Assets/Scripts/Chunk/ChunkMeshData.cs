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
  
  private readonly List<int> colliderTriangles = new();
  private readonly List<Vector3> colliderVertices = new();
  
  public ChunkMeshData transparentMeshData;
  public ChunkMeshData transparentLiquidMeshData;
  public ChunkMeshData noCullMeshData;
  public ChunkMeshData animationMeshData;
  public ChunkMeshData noCullAnimationMeshData;

  public ChunkMeshData(bool isMainMesh = true){
    this.isMainMesh = isMainMesh;
    
    if (isMainMesh){
      transparentMeshData = new ChunkMeshData(false);
      transparentLiquidMeshData = new ChunkMeshData(false);
      noCullMeshData = new ChunkMeshData(false);
      animationMeshData = new ChunkMeshData(false);
      noCullAnimationMeshData = new ChunkMeshData(false);
    }
  }
  
  public void AddTextureFace(TextureMapper.TextureMap.Face face){
    uvs.Add(face.bl);
    uvs.Add(face.tl);
    uvs.Add(face.tr);
    uvs.Add(face.br);
  }

  public void AddColors(TextureMapper.TextureMap textureMap, byte slBL, byte slTL, byte slTR, byte slBR, byte blBL, byte blTL, byte blTR, byte blBR){
    //Color32 c = textureMap.defaultColor;
    colors.Add(new Color32(0, slBL, blBL, 0));
    colors.Add(new Color32(0, slTL, blTL, 0));
    colors.Add(new Color32(0, slTR, blTR, 0));
    colors.Add(new Color32(0, slBR, blBR, 0));
  }
  
  public void AddFace(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 normal, bool collider){
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

    if (collider){
      int colliderIndex = colliderVertices.Count;
      colliderVertices.Add(a);
      colliderVertices.Add(b);
      colliderVertices.Add(c);
      colliderVertices.Add(d);
      colliderTriangles.Add(colliderIndex + 0);
      colliderTriangles.Add(colliderIndex + 1);
      colliderTriangles.Add(colliderIndex + 2);
      colliderTriangles.Add(colliderIndex + 2);
      colliderTriangles.Add(colliderIndex + 3);
      colliderTriangles.Add(colliderIndex + 0);
    }
  }

  public void SetupColliderMesh(Mesh mesh){
    // Combine vertices, including noCullAnimationMeshData collider vertices
    mesh.SetVertices(colliderVertices
        .Concat(noCullMeshData.colliderVertices) // No cull mesh collider vertices
        .Concat(transparentMeshData.colliderVertices) // Transparent mesh collider vertices
        .Concat(transparentLiquidMeshData.colliderVertices) // Liquid transparent collider vertices
        .Concat(animationMeshData.colliderVertices) // Animated collider vertices
        .Concat(noCullAnimationMeshData.colliderVertices) // No cull animation collider vertices
        .ToArray());

    // Set the triangles for the collider mesh, including noCullAnimationMeshData collider triangles
    mesh.SetTriangles(colliderTriangles
        .Concat(noCullMeshData.colliderTriangles.Select(index => index + colliderVertices.Count)) // No cull collider triangles
        .Concat(transparentMeshData.colliderTriangles.Select(index => index + colliderVertices.Count + noCullMeshData.colliderVertices.Count)) // Transparent collider triangles
        .Concat(transparentLiquidMeshData.colliderTriangles.Select(index => index + colliderVertices.Count + noCullMeshData.colliderVertices.Count + transparentMeshData.colliderVertices.Count)) // Liquid transparent collider triangles
        .Concat(animationMeshData.colliderTriangles.Select(index => index + colliderVertices.Count + noCullMeshData.colliderVertices.Count + transparentMeshData.colliderVertices.Count + transparentLiquidMeshData.colliderVertices.Count)) // Animated collider triangles
        .Concat(noCullAnimationMeshData.colliderTriangles.Select(index => index + colliderVertices.Count + noCullMeshData.colliderVertices.Count + transparentMeshData.colliderVertices.Count + transparentLiquidMeshData.colliderVertices.Count + animationMeshData.colliderVertices.Count)) // No cull animation collider triangles
        .ToArray(), 0);
  }

  public void SetupMesh(Mesh mesh){
    mesh.subMeshCount = 6; // Increment sub-mesh count for the new noCullAnimation mesh

    // Combine vertices, uvs, normals, colors, including noCullAnimationMeshData
    mesh.SetVertices(vertices
        .Concat(noCullMeshData.vertices) // No cull mesh vertices
        .Concat(transparentMeshData.vertices) // Transparent mesh vertices
        .Concat(transparentLiquidMeshData.vertices) // Liquid transparent mesh vertices
        .Concat(animationMeshData.vertices) // Animated mesh vertices
        .Concat(noCullAnimationMeshData.vertices) // No cull animation mesh vertices
        .ToArray());

    // Set triangles for each sub-mesh
    mesh.SetTriangles(triangles.ToArray(), 0); // Opaque mesh
    mesh.SetTriangles(noCullMeshData.triangles.Select(index => index + vertices.Count).ToArray(), 1); // No cull mesh
    mesh.SetTriangles(transparentMeshData.triangles.Select(index => index + vertices.Count + noCullMeshData.vertices.Count).ToArray(), 2); // Transparent mesh
    mesh.SetTriangles(transparentLiquidMeshData.triangles.Select(index => index + vertices.Count + noCullMeshData.vertices.Count + transparentMeshData.vertices.Count).ToArray(), 3); // Liquid transparent mesh
    mesh.SetTriangles(animationMeshData.triangles.Select(index => index + vertices.Count + noCullMeshData.vertices.Count + transparentMeshData.vertices.Count + transparentLiquidMeshData.vertices.Count).ToArray(), 4); // Animated mesh
    mesh.SetTriangles(noCullAnimationMeshData.triangles.Select(index => index + vertices.Count + noCullMeshData.vertices.Count + transparentMeshData.vertices.Count + transparentLiquidMeshData.vertices.Count + animationMeshData.vertices.Count).ToArray(), 5); // No cull animation mesh

    // Combine UVs for all meshes
    mesh.SetUVs(0, uvs
        .Concat(noCullMeshData.uvs) // No cull mesh uvs
        .Concat(transparentMeshData.uvs) // Transparent mesh uvs
        .Concat(transparentLiquidMeshData.uvs) // Liquid transparent mesh uvs
        .Concat(animationMeshData.uvs) // Animated mesh uvs
        .Concat(noCullAnimationMeshData.uvs) // No cull animation mesh uvs
        .ToArray());

    // Combine Normals for all meshes
    mesh.SetNormals(normals
        .Concat(noCullMeshData.normals) // No cull mesh normals
        .Concat(transparentMeshData.normals) // Transparent mesh normals
        .Concat(transparentLiquidMeshData.normals) // Liquid transparent mesh normals
        .Concat(animationMeshData.normals) // Animated mesh normals
        .Concat(noCullAnimationMeshData.normals) // No cull animation mesh normals
        .ToArray());

    // Combine Colors for all meshes
    mesh.SetColors(colors
        .Concat(noCullMeshData.colors) // No cull mesh colors
        .Concat(transparentMeshData.colors) // Transparent mesh colors
        .Concat(transparentLiquidMeshData.colors) // Liquid transparent mesh colors
        .Concat(animationMeshData.colors) // Animated mesh colors
        .Concat(noCullAnimationMeshData.colors) // No cull animation mesh colors
        .ToArray());
  }

  public void ClearCachedMeshData(){
    // Clear all the mesh data
    vertices.Clear();
    triangles.Clear();
    colors.Clear();
    uvs.Clear();
    normals.Clear();
    colliderTriangles.Clear();
    colliderVertices.Clear();

    // Clear transparent mesh data
    transparentMeshData.vertices.Clear();
    transparentMeshData.triangles.Clear();
    transparentMeshData.colors.Clear();
    transparentMeshData.uvs.Clear();
    transparentMeshData.normals.Clear();
    transparentMeshData.colliderTriangles.Clear();
    transparentMeshData.colliderVertices.Clear();

    // Clear the no cull mesh data
    noCullMeshData.vertices.Clear();
    noCullMeshData.triangles.Clear();
    noCullMeshData.colors.Clear();
    noCullMeshData.uvs.Clear();
    noCullMeshData.normals.Clear();
    noCullMeshData.colliderTriangles.Clear();
    noCullMeshData.colliderVertices.Clear();
    
    // Clear the transparent liquid mesh data
    transparentLiquidMeshData.vertices.Clear();
    transparentLiquidMeshData.triangles.Clear();
    transparentLiquidMeshData.colors.Clear();
    transparentLiquidMeshData.uvs.Clear();
    transparentLiquidMeshData.normals.Clear();
    transparentLiquidMeshData.colliderTriangles.Clear();
    transparentLiquidMeshData.colliderVertices.Clear();
    
    // Clear the animation mesh data
    animationMeshData.vertices.Clear();
    animationMeshData.triangles.Clear();
    animationMeshData.colors.Clear();
    animationMeshData.uvs.Clear();
    animationMeshData.normals.Clear();
    animationMeshData.colliderTriangles.Clear();
    animationMeshData.colliderVertices.Clear();
    
    // Clear the no cull animation mesh data
    noCullAnimationMeshData.vertices.Clear();
    noCullAnimationMeshData.triangles.Clear();
    noCullAnimationMeshData.colors.Clear();
    noCullAnimationMeshData.uvs.Clear();
    noCullAnimationMeshData.normals.Clear();
    noCullAnimationMeshData.colliderTriangles.Clear();
    noCullAnimationMeshData.colliderVertices.Clear();
  }
}