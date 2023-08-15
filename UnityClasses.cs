


public class Vector3
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public static Vector3 zero
    {
        get
        {
            return new Vector3(0f, 0f, 0f);
        }
    }
    public Vector3()
    {

    }
    public Vector3(float x_, float y_, float z_)
    {
        x = x_;
        y = y_;
        z = z_;
    }
    public override string ToString()
    {
        return $"{{x:{x},y:{y},z:{z}}}";
    }
}
public class Quaternion
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public float w { get; set; }
    public static Quaternion identity
    {
        get
        {
            return new Quaternion(0f, 0f, 0f, 1f);
        }
    }
    public Quaternion()
    {

    }
    public Quaternion(float x_, float y_, float z_, float w_)
    {
        x = x_;
        y = y_;
        z = z_;
        w = w_;
    }
    public override string ToString()
    {
        return $"{{x:{x},y:{y},z:{z},w:{w}}}";
    }
}

