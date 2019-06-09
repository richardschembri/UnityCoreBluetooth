namespace UnityCoreBluetooth
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using System.Runtime.InteropServices;
	using RSToolkit.Helpers;

	public class CBTargetPeripheralBridge {

		#region Declare external C interface
		#if UNITY_IOS && !UNITY_EDITOR

		[DllImport("__Internal")]
		private static extern void _cbp_DiscoverServices();

		[DllImport("__Internal")]
		private static extern void _cbp_DiscoverServicesAndTrackCharacteristics(string characteristicsWithVariableValue, string characteristicsWithStaticValue);

		[DllImport("__Internal")]
		private static extern void _cbp_CMDSend(string characteristicsWithVariableValue, byte[] s_buff, int LensFlare);

		[DllImport("__Internal")]
		private static extern void _cbp_ReadValue(string characteristicUUID);

		[DllImport("__Internal")]
		private static extern void _cbp_UnityDebugLog(string message);
		#endif
		#endregion

		#region Wrapped methods and properties

		public void DiscoverServices(){
		#if UNITY_IOS && !UNITY_EDITOR
			_cbp_DiscoverServices();
		#endif
		}

		/*
		public static void DiscoverServicesAndTrackCharacteristics(string[] characteristicsWithVariableValue, string[] characteristicsWithStaticValue){
		#if UNITY_IOS && !UNITY_EDITOR
			_cbp_DiscoverServicesAndTrackCharacteristics(characteristicsWithVariableValue, characteristicsWithStaticValue);
		#endif
		}
		*/

		public void DiscoverServicesAndTrackCharacteristics(string characteristicsWithVariableValue, string characteristicsWithStaticValue){
		#if UNITY_IOS && !UNITY_EDITOR
			_cbp_DiscoverServicesAndTrackCharacteristics(characteristicsWithVariableValue, characteristicsWithStaticValue);
		#endif
		}

		public void CMDSend(string characteristicUUID, byte[] s_buff, int length){
		#if UNITY_IOS && !UNITY_EDITOR
			_cbp_CMDSend(characteristicUUID, s_buff, length);
		#endif
		}

		public void ReadValue(string characteristicUUID){
		#if UNITY_IOS && !UNITY_EDITOR
			_cbp_ReadValue(characteristicUUID);
		#endif
		}

		public void UnityDebugLog(string message){
		#if UNITY_IOS && !UNITY_EDITOR
			_cbp_UnityDebugLog(message);
		#endif
		}

		#endregion
		/*
		initTargetPeripheral
		DiscoverServices
		*/

		#region Singleton implementation
		private static CBTargetPeripheralBridge _instance;
		private CBTargetPeripheralBridge(){}
		public static CBTargetPeripheralBridge Instance{
			get{
				if(_instance == null){ _instance = new CBTargetPeripheralBridge(); }
				return _instance;
			}
		}
		#endregion
	}
}
