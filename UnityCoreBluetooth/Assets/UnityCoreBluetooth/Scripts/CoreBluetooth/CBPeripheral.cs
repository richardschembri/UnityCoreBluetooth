namespace UnityCoreBluetooth
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using System.Reflection;

    public class CBPeripheral {

        public string name {get; private set;}
        public string uuid {get; private set;}

        public enum CBPeripheralState
        {
            noState,
            poweredOff,
            poweredOn,
            resetting,
            unauthorized,
            unknown,
            unsupported,
        }
        private void Init(string name, string uuid){
            this.name = name;
            this.uuid = uuid;
        }
        public CBPeripheral(string name, string uuid){
            Init(name, uuid);
        }
        public CBPeripheral(string jsonDataString){

            var cbPeripheralJSON = JSON.Parse(jsonDataString);
            Init( cbPeripheralJSON["name"].Value,
                    cbPeripheralJSON["uuid"].Value);

            //cbPeripheralJSON.ToJSON(0);
        }
        public string ToJSON(){
            var jsonFormat = "{{ name:\"{0}\",uuid:\"{1}\"}}";
                return string.Format(jsonFormat, name, uuid);
        }
        public void Disconnect() {
            CBCentralManagerBridge.Instance.CancelPeripheralConnection(this.name);
        }
    }
    [AttributeUsage(AttributeTargets.Field)]
    public class StringValueByEnum : Attribute
    {
        private readonly string _value;
        public StringValueByEnum(string value) { _value = value; }
        public string Value { get { return _value; } }
        public static string GetStringByEnum(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            StringValueByEnum[] attrs = fi.GetCustomAttributes(typeof(StringValueByEnum), false) as StringValueByEnum[];
            return attrs[0].Value;
        }
    }
}