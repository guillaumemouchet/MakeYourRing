# MakeYourRing

MakeYourRing is a Bachelor Project made by Guillaume Mouchet, it's an application to help users create different types of jewels in augmented reality.<br>
You'll need a computer to run the application, and a Hololens 2 to create a remote access to it.<br>
You'll be able to import your own asset to work on them, save you creation and delete what you don't like.<br>
You can also take the different parts and fuse them together, you'll always have a leader to help you move all of them at once.

One thing to take note of is that you can for know only use ".obj" object to import, and those need to be on your computer, the Hololens 2 is only used like a "touch screen".

## Welcome

MakeYourRing is a projet made with Unity and is made to work on Hololens 2.

You'll find many informations in the wiki, "Recherche et Documentation" gives informations on how to build a project from scratch or how to clone and continue this project.

Many diffrent branch exist, the important ones are :

- POC (Proof of Concept), it's the first iteration of the project to show that it was possible to acces file system and many other things on the Hololens 2
- main, you'll find the finished version on this branch
- all the other branch are for developpment purpous only

If you are intriged on how and when I worked, and all the problems I encontered, you can go see "Journal de Bord", where I try to say and show on what i'm working on currently.

## How to clone

1. first of all you'll need [<span dir="">Unity Hub</span>](https://unity.com/unity-hub) with this specific version [<span dir="">2020.3.45</span>](https://unity.com/releases/editor/whats-new/2020.3.45)<span dir=""> LTS</span>
2. you have to clone the [projet](https://gitlab-etu.ing.he-arc.ch/isc/2022-23/niveau-3/3285-tb-il/276/MakeYourRing/-/tree/POC?ref_type=heads) with the following command \
   \`\`\`git clone git@gitlab-etu.ing.he-arc.ch:isc/2022-23/niveau-3/3285-tb-il/276/MakeYourRing.git\`\`\`
3. Chack that you are on the wanted branch, main is the release version
4. In Unity Editor you need to go on "Open" and select your project, your project is defined in the folder "Asset" in the hierarchy.
5. The importation can take a while
6. Errors can appear with 3ds Max, once the projet builded or tested the errors will be gone.
7. The projet in on the MainScene scene
8. In case of any other errors go see in the "Recherche et documentation" the way to create a project with "PC Remoting", it might help to see if all the players settings are the same and if MRTK is well installed.

![Logo He-arc](logoTransHeArc.png)
He-arc tous droits réservé.
