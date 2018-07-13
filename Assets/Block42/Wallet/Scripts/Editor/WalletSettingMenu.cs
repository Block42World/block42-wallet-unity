using UnityEngine;
using UnityEditor;

namespace Block42
{
	
	public static class WalletSettingMenu
	{

		[MenuItem("Block42/Wallet/Settings")]
        public static void WalletSettings()
		{
            WalletSettings settings = Resources.Load<WalletSettings>("WalletSettings");

			if (settings == null) {
                settings = ScriptableObject.CreateInstance<WalletSettings>();
                string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Block42/Resources/WalletSettings.asset");
                AssetDatabase.CreateAsset(settings, assetPathAndName);
                Debug.Log("Creating WalletSettings asset at " + assetPathAndName);
			}

			Selection.activeObject = settings;
		}
	}

}