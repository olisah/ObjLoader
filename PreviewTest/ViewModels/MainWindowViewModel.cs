﻿using Livet;
using Livet.Commands;
using Loader;
using Microsoft.Win32;
using PreviewTest.Extensions;
using PreviewTest.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PreviewTest.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public string PreviewModelPath
        {
            get => _PreviewModelPath;
            set => RaisePropertyChangedIfSet(ref _PreviewModelPath, value);
        }
        string _PreviewModelPath = string.Empty;

        public GeometryModel3D PreviewModel
        {
            get => _PreviewModel;
            set => RaisePropertyChangedIfSet(ref _PreviewModel, value);
        }
        GeometryModel3D _PreviewModel = null;

        public PerspectiveCamera Camera
        {
            get => _Camera;
            set => RaisePropertyChangedIfSet(ref _Camera, value);
        }
        PerspectiveCamera _Camera = null;

        public Point3D CameraPosition
        {
            get => _CameraPosition;
            set => UpdateCameraPosition(value);
        }
        Point3D _CameraPosition = new Point3D(0, 5, -15);

        public Vector3D CameraLookAt
        {
            get => _CameraLookAt;
            set => UpdateCameraLookAt(value);
        }
        Vector3D _CameraLookAt = new Vector3D(0, 0, 0);

        public Vector3D CameraUpDirection
        {
            get => _CameraUpDirection;
            set => UpdateCameraUpDirection(value);
        }
        Vector3D _CameraUpDirection = new Vector3D(0, 1, 0);

        public double CameraDistance
        {
            get => _CameraDistance;
            set => UpdateCameraDistance(value);
        }
        double _CameraDistance = 10;


        public ViewModelCommand LoadedCommand => _LoadedCommand.Get(Loaded);
        ViewModelCommandHandler _LoadedCommand = new ViewModelCommandHandler();

        public ViewModelCommand ExplorerCommand => _ExplorerCommand.Get(Explorer);
        ViewModelCommandHandler _ExplorerCommand = new ViewModelCommandHandler();

        public ViewModelCommand ResetCameraCommand => _ResetCameraCommand.Get(ResetCamera);
        ViewModelCommandHandler _ResetCameraCommand = new ViewModelCommandHandler();

        Point3D _ResetPosition = new Point3D();
        Vector3D _ResetLookAt = new Vector3D();
        Vector3D _ResetUpDirection = new Vector3D();

        public MainWindowViewModel()
        {        

        }

        void Loaded()
        {
            // developing test.
            PreviewModel = GeometryMaker.MakeCube3D();

            _ResetPosition = CameraPosition;
            _ResetLookAt = CameraLookAt;
            _ResetUpDirection = CameraUpDirection;

            var nCameraLookDir = (CameraLookAt - CameraPosition).NormalizeToVector3D();
            Camera = new PerspectiveCamera(CameraPosition, nCameraLookDir, CameraUpDirection, 45);

            UpdateCamera();
        }

        void Explorer()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "OBJ file(*.obj)|*.obj";
            ofd.Title = "open to preview obj file.";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == false)
            {
                return;
            }

            PreviewModelPath = ofd.FileName;

            var handle = ObjLoader.CreateHandle();
            var result = ObjLoader.Load(handle, PreviewModelPath);
            if(result == false)
            {
                // TODO: error handling.
                return;
            }

            var objMesh = new MeshGeometry3D()
            {
                Positions = new Point3DCollection(handle.Vertices.Select(arg => arg.Position.ToPoint3D()))
            };

            PreviewModel = new GeometryModel3D()
            {
                Geometry = objMesh,
                Material = new DiffuseMaterial(Brushes.Gray)
            };
        }

        void ResetCamera()
        {
            CameraPosition = _ResetPosition;
            CameraLookAt = _ResetLookAt;
            CameraUpDirection = _ResetUpDirection;
        }

        void UpdateCameraPosition(Point3D value)
        {
            _CameraPosition = value;
            RaisePropertyChanged(nameof(CameraPosition));
            UpdateCamera();
        }

        void UpdateCameraLookAt(Vector3D value)
        {
            _CameraLookAt = value;
            RaisePropertyChanged(nameof(CameraLookAt));
            UpdateCamera();
        }

        void UpdateCameraUpDirection(Vector3D value)
        {
            _CameraUpDirection = value;
            RaisePropertyChanged(nameof(CameraUpDirection));
            UpdateCamera();
        }

        void UpdateCameraDistance(double value)
        {
            _CameraDistance = value;
            RaisePropertyChanged(nameof(CameraDistance));
            UpdateCamera();
        }

        void UpdateCamera()
        {
            if (Camera != null)
            {
                var nCameraLookDir = (CameraLookAt - CameraPosition).NormalizeToVector3D();
                Camera.Position = CameraPosition;
                Camera.LookDirection = nCameraLookDir;
                Camera.UpDirection = CameraUpDirection;
            }
        }
    }
}
