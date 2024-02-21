# SEGAR: Social Environment Generator for Agent-based Research

This repository contains a tool that facilitates the creation of a virtual environment for the development of agent-based models, especially in the context of computational social sciences. It uses geographic data of any kind of political divisions as a basis and allows the loading of other relevant data such as the location of schools, universities, shops, etc.

Nothing prevents the tool from being used with other types of models and geographical divisions. For example, rooms in a hotel, laboratories in a research centre or relevant areas within a warehouse.

It is fully compatible and intended to be used together with the Agent-Based Modelling Framework for Unity3d ([ABMU](https://github.com/cheliotk/unity_abm_framework)).

## Installation
### From Github

It is possible to clone the GitHub repository locally, open the project with the Unity Hub, view the sample scenes and start building the model from the repository itself. If you want to import the tool into an existing project, you will need to copy the Scripts, Material and Prefabs folders into the new project.

## Examples
When downloading the project inside the Assets/Examples folder there are two folders A Coruña and Vitoria that contain an example scene to test and see how the environment works. The Vitoria example requires commenting on the creation of relevant nodes in the "EnvironmentGenerator" script.

* A Coruña with relevant nodes and buildings. To see this example, you need to enter the A Coruña scene. In this scene, the program simply loads the census sections, generates empty agents distributed in the census sections according to the socio-demographic data, and adds the supermarkets of the city as relevant places.
* Vitoria. To see this example, enter the Vitoria folder and start a simulation. Similar to A Coruña, the example loads the census tracts of Vitoria along with the buildings of the city. For this example to work, it is necessary to disable the corresponding node generation function in the EnvironmentGenerator script.

## Licensing
The code in this project is licensed under [MIT license](https://github.com/alejandrorodriguezarias/VirtualEnvironmentComputationalSocialScience/blob/main/LICENSE).
