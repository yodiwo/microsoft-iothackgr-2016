# microsoft-iothackgr-2016
Source code and documentation for the Microsoft IOT Hackathon GR 2016

Cloud2Cloud Intercommunication- Microsoft Azure IOT Hub with Yodiwo Cloud Platform
===
---

# Table of Contents

-   [Step 1: Set up a Raspberry Pi 2 device, equipped with Grove Sensors, to the Microsoft Azure IOT hub](#Step-1)
-   [Step 2: Interconnect Microsoft Azure IOT hub with Yodiwo Cloud Platform](#Step-2)
-   [Step 3: Set up a Raspberry PI 2 device equipped with SkyWriter Hat,to the Yodiwo Cloud](#Step-3)
-   [Step 4: Set up a Windows Phone to the Yodiwo Cloud Platform](#Step-4)
-   [Demos: Create use case scenarios to the Yodiwo Cloud Platform](#Demos)
-   [Tips](#tips)

<a name="Step-1"></a>
# Step 1: Set up a Raspberry Pi 2 device, equipped with Grove Sensors, to the Microsoft Azure IOT hub

1. Create an Azure IOT hub following these instructions: 
https://azure.microsoft.com/en-us/documentation/articles/iot-hub-csharp-csharp-getstarted/
Make a note of the AzureIOTHub HostName and iot hub connection string.

2. Create a Device Identity using the Microsoft Device Explorer Application: https://github.com/Azure/azure-iot-sdks/releases
(Scroll down for SetupDeviceExplorer.msi)
For more information, have a look at: https://github.com/Azure/azure-iot-sdks/blob/master/tools/DeviceExplorer/doc/how_to_use_device_explorer.md (Section Create Devices).  Click “SAS Token...” to generate a device specific connection string with SAS token and Get device connection string. Make a note of the device id and device connection string.

3.	Use a Raspberry PI 2, running Windows IOT Core. Connect the Grove extension board on the Raspberry device and attach the Grove sensors to the following slots:
Rotary Angle Sensor: A2
Button: D3
Light: D4
Led: D5
LCD: any I2C slot

4.	From Visual Studio, open the Yodiwo.MicrosoftHackathon.sln and go to the Project App1. Open MainPage.xaml.cs. Change RaspName (Line36) and Connection String (Line 44) with the device id and the device connection string respectively, acquired previously in substep 2.
	
5.	Build the Universal Project and Deploy it on the Raspberry PI.





<a name="Step-2"></a>
# Step 2: Interconnect Microsoft Azure IOT hub with Yodiwo Cloud Platform

<a name="Step-3"></a>
# Step 3: Set up a Raspberry PI 2 device equipped with SkyWriter Hat,to the Yodiwo Cloud

<a name="Step-4"></a>
# Step 4: Set up a Windows Phone to the Yodiwo Cloud Platform

<a name="Demos"></a>
# Demos: Create use case scenarios to the Yodiwo Cloud Platform

<a name="tips"></a>
# Tips
