# PatLB (Panic at the [elastic] LoadBalancer) - WORK IN PROGRESS

The intention of this toolkit is to give a System Operator the ability to do the following:
- Optionally bring a selected EC2 instance off of an Elastic Load Balancer (ELB) Target Group
- Optionally, post Target Group removal, execute a house cleaning script.
- Execute the provisioning script that will bring the EC2 instance to a new "healthy" state
  - If healthy, continue
  - If unhealthy, do not continue, alert the System Operator and await input
- Rejoin the EC2 instance to the original (or optionally different) ELB Target Group

## Building and "Packaging" the Project
Requirements:
- Only works on Windows.
- Needs .NET 6.0 Framework and Visual Studio 2022 on at least Windows 10

If you've met the requirements: execute Packer.ps1 in Windows PowerShell (Optionally ISE).
