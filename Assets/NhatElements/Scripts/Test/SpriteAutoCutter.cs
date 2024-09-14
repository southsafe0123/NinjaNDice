// using UnityEngine;
// using UnityEditor;

// public class SpriteAutoCutter : MonoBehaviour
// {
//     // [MenuItem("Assets/Auto Cut Sprite")]
//     static void AutoCutSprite()
//     {
//         // Lấy đường dẫn của sprite đã cắt
//         string originalPath = AssetDatabase.GetAssetPath(Selection.activeObject);
//         if (string.IsNullOrEmpty(originalPath))
//         {
//             Debug.LogError("No original sprite selected.");
//             return;
//         }

//         // Thông tin về sprite đã cắt
//         TextureImporter originalImporter = AssetImporter.GetAtPath(originalPath) as TextureImporter;
//         if (originalImporter == null)
//         {
//             Debug.LogError("Failed to get original sprite importer.");
//             return;
//         }

//         // Lấy đường dẫn của sprite mới
//         string newSpritePath = EditorUtility.OpenFilePanel("Select new sprite", "", "png");
//         if (string.IsNullOrEmpty(newSpritePath))
//         {
//             Debug.LogError("No new sprite selected.");
//             return;
//         }

//         // Copy file vào thư mục Assets
//         string newAssetPath = "Assets/" + System.IO.Path.GetFileName(newSpritePath);
//         System.IO.File.Copy(newSpritePath, newAssetPath, true);
//         AssetDatabase.Refresh();

//         // Thông tin về sprite mới
//         TextureImporter newImporter = AssetImporter.GetAtPath(newAssetPath) as TextureImporter;
//         if (newImporter == null)
//         {
//             Debug.LogError("Failed to get new sprite importer.");
//             return;
//         }

//         // Sao chép các thuộc tính từ sprite cũ sang sprite mới
//         newImporter.spriteImportMode = originalImporter.spriteImportMode;
//         newImporter.spritePixelsPerUnit = originalImporter.spritePixelsPerUnit;
//         newImporter.mipmapEnabled = originalImporter.mipmapEnabled;
//         newImporter.filterMode = originalImporter.filterMode;
//         newImporter.textureType = originalImporter.textureType;
//         newImporter.spritePivot = originalImporter.spritePivot;

//         // Sao chép các cắt sprite
//         if (originalImporter.spriteImportMode == SpriteImportMode.Multiple)
//         {
//             SpriteMetaData[] originalMetaData = originalImporter.spritesheet;
//             SpriteMetaData[] newMetaData = new SpriteMetaData[originalMetaData.Length];
//             for (int i = 0; i < originalMetaData.Length; i++)
//             {
//                 newMetaData[i] = originalMetaData[i];
//             }
//             newImporter.spritesheet = newMetaData;
//         }

//         // Áp dụng thay đổi
//         AssetDatabase.ImportAsset(newAssetPath, ImportAssetOptions.ForceUpdate);
//         Debug.Log("Sprite auto cut and imported successfully.");
//     }
// }
