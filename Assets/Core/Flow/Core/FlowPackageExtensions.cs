using LiteNetLib.Utils;
using UnityEngine;


public static class FlowPackageExtensionVector2 {

  public static void Put(this NetDataWriter writer, Vector2 vector) {
    writer.Put(vector.x);
    writer.Put(vector.y);
  }

  public static Vector2 GetVector2(this NetDataReader reader) {
    Vector2 v;
    v.x = reader.GetFloat();
    v.y = reader.GetFloat();
    return v;
  }
}
public static class FlowPackageExtensionVector3 {
  public static void Put(this NetDataWriter writer, Vector3 vector) {
    writer.Put(vector.x);
    writer.Put(vector.y);
    writer.Put(vector.z);
  }

  public static Vector3 GetVector3(this NetDataReader reader) {
    Vector3 v;
    v.x = reader.GetFloat();
    v.y = reader.GetFloat();
    v.z = reader.GetFloat();
    return v;
  }
}
public static class FlowPackageExtensionQuarterion {
  public static void Put(this NetDataWriter writer, Quaternion vector) {
    writer.Put(vector.x);
    writer.Put(vector.y);
    writer.Put(vector.z);
    writer.Put(vector.w);
  }

  public static Quaternion GetQuaternion(this NetDataReader reader) {
    Quaternion v;
    v.x = reader.GetFloat();
    v.y = reader.GetFloat();
    v.z = reader.GetFloat();
    v.w = reader.GetFloat();
    return v;
  }
}