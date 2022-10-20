using System.Collections;
using UnityEngine;
using Google.XR.ARCoreExtensions;
using UnityEngine.XR.ARSubsystems;
using System;

public class PositionSensor : MonoBehaviour 
{
    private double _nowLat;
    private double _nowLong;
    private GeospatialPose _pose;
    private bool _gpsStarted = false;
    private bool _gpsOn = false;
 
    private WaitForSeconds _retry;
    private LocationInfo _location;
    [SerializeField] private float _retryTime;
    [SerializeField] private AREarthManager _earth;
 
    /// <summary>
    /// 매개변수로 true, false를 넣어서 On/Off 처리
    /// </summary>
    public void GpsSwitch(bool Select)
    {
        if(Select && _gpsOn == false)
        {
            StartCoroutine(GpsStart());
            _gpsOn = true;
        }
        else if(!Select && _gpsOn == true)
        {
            GpsStop();
            _gpsOn = true;
        }

    }

    private void Start() 
    {
        _retry = new WaitForSeconds (_retryTime);

        StartCoroutine(GpsStart());
    }

    private IEnumerator GpsStart() 
    {
        if (!Input.location.isEnabledByUser) 
        {
            yield break;
        }
 
        Input.location.Start ();
 
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) 
        {
            yield return _retry;
            maxWait -= 1;
        }

        if (maxWait < 1)
        {
            yield break;
        }
 
        if (Input.location.status == LocationServiceStatus.Failed) 
        {
            yield break;
 
        } 
        else 
        {
            _gpsStarted = true;

            while (_gpsStarted) 
            {
                NowPositionSet();
                AzimuthSet();
                yield return _retry;
            }
        }
    }

    private void NowPositionSet()
    {
        _location = Input.location.lastData;
        _nowLat = _location.latitude * 1.0d;
        _nowLong = _location.longitude * 1.0d;
    }

    private void AzimuthSet()
    {
        _pose = _earth.EarthState == EarthState.Enabled && 
        _earth.EarthTrackingState == TrackingState.Tracking ?
        _earth.CameraGeospatialPose : new GeospatialPose();
    }
 
    public void GpsStop() 
    {
        if (Input.location.isEnabledByUser) 
        {
            _gpsStarted = false;
            Input.location.Stop();
            StopCoroutine(GpsStart());
        }
    }



    public double GetLat()
    {
        return _nowLat;
    }

    public double GetLong()
    {
        return _nowLong;
    }

    public Vector2 GetEarthPos()
    {
        return new Vector2((float)GetLat(), (float)GetLong());
    }

    public double GetAzimuth()
    {
        return _pose.Heading;
    }

    /// <summary>
    /// 위/경도 기준 A에서 B까지의 거리 반환(구체 표면거리, 미터(M) 기준)
    /// </summary>
    public double GetDistAtoB(double A_Lat, double A_Long, double B_Lat, double B_Long)
    {
        double Theta;
        double Distance;

        Theta = A_Long - B_Long;
        
        Distance = Math.Sin(deg2rad(A_Lat)) * Math.Sin(deg2rad(B_Lat)) + Math.Cos(deg2rad(A_Lat)) * Math.Cos(deg2rad(B_Lat)) * Math.Cos(deg2rad(Theta));
        Distance = Math.Acos(Distance);
        Distance = rad2deg(Distance);

        Distance = Distance * 60 * 1.1515;
        Distance = Distance * 1.609344;
        Distance = Distance * 1000.0;

        return Distance;
    }

    private double deg2rad(double deg)
    {
        return (double)(deg * Math.PI / (double)180d);
    }
    private double rad2deg(double rad)
    {
        return (double)(rad * (double)180d / Math.PI);
    }
}