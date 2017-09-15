using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace App
{
    public class Crane
    {
        public string CraneNo { get; set; }
        public string TaskNo { get; set; }
        public object[] Status { get; set; }

        public bool Mode { get; set; }
        public bool ForkStatus { get; set; }
        public int AlarmCode { get; set; }
    }

    public delegate void CraneEventHandler(CraneEventArgs args);
    public class CraneEventArgs
    {
        private Crane _crane;
        public Crane crane
        {
            get
            {
                return _crane;
            }
        }
        public CraneEventArgs(Crane crane)
        {
            this._crane = crane;
        }
    }
    public class Cranes
    {
        public static event CraneEventHandler OnCrane = null;

        public static void CraneInfo(Crane crane)
        {
            if (OnCrane != null)
            {
                OnCrane(new CraneEventArgs(crane));
            }
        }
    }

    public class Car
    {
        public string CarNo { get; set; }
        public object[] Status { get; set; }
        public string TaskNo { get; set; }
        public int AlarmCode { get; set; }
    }

    public delegate void CarEventHandler(CarEventArgs args);
    public class CarEventArgs
    {
        private Car _car;
        public Car car
        {
            get
            {
                return _car;
            }
        }
        public CarEventArgs(Car car)
        {
            this._car = car;
        }
    }
    public class Cars
    {
        public static event CarEventHandler OnCar = null;

        public static void CarInfo(Car car)
        {
            if (OnCar != null)
            {
                OnCar(new CarEventArgs(car));
            }
        }
    }
    public class Conveyor
    {
        public string ctlName { get; set; }
        public string ID { get; set; }
        public string value { get; set; }
        public string BarCode { get; set; }
    }

    public delegate void ConveyorEventHandler(ConveyorEventArgs args);
    public class ConveyorEventArgs
    {
        private Conveyor _conveyor;
        public Conveyor conveyor
        {
            get
            {
                return _conveyor;
            }
        }
        public ConveyorEventArgs(Conveyor conveyor)
        {
            this._conveyor = conveyor;
        }
    }
    public class Conveyors
    {
        public static event ConveyorEventHandler OnConveyor = null;

        public static void ConveyorInfo(Conveyor conveyor)
        {
            if (OnConveyor != null)
            {
                OnConveyor(new ConveyorEventArgs(conveyor));
            }
        }
    }
    public class Miniload
    {
        public string MiniloadNo { get; set; }
        public object Status { get; set; }
        public object[] Station { get; set; }
        public int Mode { get; set; }
        public int ForkStatus { get; set; }
        public string TaskANo { get; set; }
        public string TaskBNo { get; set; }
        public int AlarmCode { get; set; }
    }

    public delegate void MiniloadEventHandler(MiniloadEventArgs args);
    public class MiniloadEventArgs
    {
        private Miniload _miniload;
        public Miniload miniload
        {
            get
            {
                return _miniload;
            }
        }
        public MiniloadEventArgs(Miniload miniload)
        {
            this._miniload = miniload;
        }
    }
    public class Miniloads
    {
        public static event MiniloadEventHandler OnMiniload = null;

        public static void MiniloadInfo(Miniload miniload)
        {
            if (OnMiniload != null)
            {
                OnMiniload(new MiniloadEventArgs(miniload));
            }
        }
    }
}
