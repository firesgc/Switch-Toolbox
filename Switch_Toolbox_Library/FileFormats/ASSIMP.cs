using System.IO;
using Assimp;

namespace Toolbox.Library
{
    public class ASSIMP
    {
        private enum ExportFormat
        {
            None = -1,

            GLTF = 10,

            // doesnt work ... dunno GLTF_BIN = 11,
            GLTF2 = 12,

            ASSIMP = 13,
            OBJ = 3,
            OBJ_NO_MTL = 4,

            DAE = 0,
            Count
        }

        public static string WriteScene(Scene aScene, string aExportFile)
        {
            var ourAssimp = new AssimpContext();
            var formatIds = ourAssimp.GetSupportedExportFormats();
            var outName = Path.ChangeExtension(aExportFile, ".gltf");

            ourAssimp.ExportFile(aScene, outName, formatIds[(int)ExportFormat.GLTF2].FormatId, PostProcessSteps.None);

            return outName;
        }


        public static void ExportMesh(string FileName, STGenericObject genericMesh)
        {
            var scene = new Scene();

            scene.RootNode = new Node(Path.GetFileNameWithoutExtension(FileName));

            var node = new Node(genericMesh.Name);
            scene.RootNode.Children.Add(node);

            var aiMesh = new Mesh();

            foreach (var v in genericMesh.vertices)
            {
                aiMesh.Vertices.Add(new Vector3D(v.pos.X, v.pos.Y, v.pos.Z));
                aiMesh.Normals.Add(new Vector3D(v.nrm.X, v.nrm.Y, v.nrm.Z));

                aiMesh.TextureCoordinateChannels[0].Add(new Vector3D(v.uv0.X, v.uv0.Y, 0));
                aiMesh.TextureCoordinateChannels[1].Add(new Vector3D(v.uv1.X, v.uv1.Y, 0));
                aiMesh.TextureCoordinateChannels[2].Add(new Vector3D(v.uv2.X, v.uv2.Y, 0));

                aiMesh.VertexColorChannels[0].Add(new Color4D(v.col.X, v.col.Y, v.col.Z, v.col.W));
                aiMesh.VertexColorChannels[1].Add(new Color4D(v.col2.X, v.col2.Y, v.col2.Z, v.col2.W));
            }

            aiMesh.UVComponentCount[0] = 2;
            aiMesh.UVComponentCount[1] = 2;
            aiMesh.UVComponentCount[2] = 2;

            for (var index = 0; index < genericMesh.lodMeshes[0].faces.Count; index += 3)
            {
                var indices = new int[3];
                indices[0] = genericMesh.lodMeshes[0].faces[index];
                indices[1] = genericMesh.lodMeshes[0].faces[index + 1];
                indices[2] = genericMesh.lodMeshes[0].faces[index + 2];
                aiMesh.Faces.Add(new Face(indices));
            }

            var mat = new Material
            {
                ColorDiffuse = new Color4D(0.5f, 0.5f, 0.5f, 1),
                Name = "DefaultMaterial"
            };

            aiMesh.MaterialIndex = scene.Materials.Count;
            scene.Materials.Add(mat);

            // Add mesh to root
            node.MeshIndices.Add(scene.Meshes.Count);
            scene.Meshes.Add(aiMesh);

            WriteScene(scene, FileName);
        }
    }
}