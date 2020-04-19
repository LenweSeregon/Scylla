#Scene management

Scylla permet de gérer simplement la transition de scène via le module "SceneManagement".

Attention, cette documentation ne présentera pas le fonctionnement des méthodes des différents classes mais présente plus 
simplement le fonctionnement général de Scylla pour la gestion des scènes.

La gestion de scène de Scylla repose sur le chargement additif de scène. En effet, comme préconisé dans la documentation
d'Unity, il est préfèrable d'éviter la fonction "DontDestroyOnLoad". Ainsi, Scylla s'appuie sur une scène principale
obligatoire qui contiendra les managers et les objets devant perdurer durant tout le long de l'utilisation de l'application.

Cette première scène contenant au moins le SceneManager ne peut être supprimée et le gestionnaire s'assurera de ne pas laisser
sa suppression même si l'utilisateur commet une erreur en le demandant. Ensuite, les différentes scènes seront ajoutées de manière
additive. Il est bien-sûr aussi possible de supprimer des scènes ajoutées de manière additive.

## SceneManager

Le SceneManager est le point d'entrée pour communiquer avec le système puisqu'il est le seul singleton à contenir une référence 
vers le proxy qui permet de faire le lien entre le fonctionnement interne du système et les demandes de changement de scène.

## Principes de gestion & InternalSceneData
Avant de voir le fonctionnement interne, il faut présenter la classe InternalSceneData qui est la représentation d'un classe selon Scylla
ainsi que les principes utilisés par Scylla pour sa gestion.

- **Bundle** : Toutes les scènes de Scylla sont encapsulées dans un bundle. Cela permet dans un premier temps de charger plusieurs scènes en même temps et de pouvoir les décharger en même temps.  
L'utilisateur peut choisir de définir un nom de bundle lui même ou laisser Scylla en définir un pour lui. Il est aussi possible de définir dans le bundle que l'on souhaite charger, les scènes
qui sont considérées comme principales dans le bundle (permettant de supprimer un bundle sauf les scènes principales par exemple).
- **Marked** : On utilise aussi le principe de scène Marked. Si l'utilisateur défini sa scène comme Marked, il sera possible de demander de
décharger toutes les scènes jusqu'à une scène Marked. Cela peut être utile si l'on souhaite avoir des scènes agissantes comme gestionnaire de sous scène.  
Exemple :  
```
On demande a charger le bundle suivant :
SceneLevelLoader => Marked
SceneLevel1 => Not Marked

On pourra facilement demander ensuite :
Load SceneLevel2 and UnloadUntilMarked

On aura ainsi : 
SceneLevelLoader
SceneLevel2
```
-  **Suppressible** : Finalement, il est possible de définir si notre scène est suppressible ou non. Lorsqu'une scène n'est pas suppressible
et qu'un utilisateur tente de la supprimer sans forcer, la requête est rejetée.

![GitHub Logo](Images/SceneDataInternalFields.png)

Il reste le bool pour savoir si une scène est considérée comme la scène principale (la scène contenant le SceneManager).
Cette donnée est utilisée en interne pour s'assurer qu'on ne détruit pas la scène principale.

## SceneLoader

Le SceneLoader est une classe interne ne pouvant pas être accédée en dehors de Scylla car c'est le SceneLoaderProxy qui joue 
ce rôle d'intermédiaire. Le fonctionnement du SceneLoader est assez simple et s'appuie sur 3 classes internes.

### SceneLoaderEvents
Tout d'abord, afin d'obtenir une gestion modulable et d'avoir le moins de dépendance possible, Scylla utilise les événements C#
afin de communiquer la progression d'un opération de modification de scène.  
Il y a actuellement 5 événements déclenchés par le SceneLoader :
- **Action OnLoaderProcessStart** : Avertir du commencement d'un processus (chargement / déchargement de scène(s))
- **Action OnLoaderProcessFinish** : Avertir de la fin d'un processus (chargement / déchargement de scène(s))
- **Action<string, float> OnLoaderProcessUpdate** : Avertir de la progression d'un processus  
Le string représente une chaine de caractère pour savoir ce qu'il se passe actuellement dans le chargement  
Le float représente la progression globale du processus
- **Action<string> OnSceneLoaded** : Avertir de la complétion du chargement d'un scène  
Le string représente le nom de la scène chargée
- **Action<string> OnSceneUnloaded** : Avertir de la complétion d'un déchargement d'une scène  
Le string représente le nom de la scène déchargée

### SceneLoaderProcess
Le SceneLoaderProcess est la classe qui permet de gérer une suite de requêtes, il s'assure d'appeler les opérations de chargement / déchargement, 
de fournir la progression globale du processus.
Afin de pouvoir avoir un écran de chargement qui s'affiche lorsque le chargement est trop rapide, l'inspecteur du SceneLoader permet de définir un temps minimal
de chargement qui sera passé au SceneLoaderProcess.
Le SceneLoaderProcess s'occupe de gérer séquentiellement le chargement / déchargement en vidant la queue de requête **InternalSceneRequest** qui lui est fourni.


### SceneCollection

La SceneCollection est simplement un conteneur de données. C'est elle qui va permettre d'attribuer automatiquement les identifiants de bundle lorsque ces
derniers ne sont pas spécifiés par les utilisateurs.

C'est aussi elle qui sera interrogée par le **SceneLoaderProxy** pour savoir si une requête est valide et pour obtenir des informations de scène lorsque les utilisateur le demandent.  
Lorsqu'une scène est chargée ou déchargée, les **InternalSceneRequest** se chargeront d'appeler une méthode pour avertir la collection. Lors d'un déchargement de scène, la 
SceneCollection s'occupera de récupérer son identifiant de bundle pour le réutiliser si celui-ci match avec le pattern qui est automatiquement attribué.

## SceneLoaderProxy

Le SceneLoaderProxy est le seul point de communication avec le système de gestion de scène de Scylla. Il va ainsi récupérer toutes les requêtes utilisateur demandant à charger / supprimer des scènes.
Dans l'inspecteur du **SceneLoaderProxy**, il pourra être défini si l'on souhaite que la nouvelle requête efface les anciennes (via **AttemptClearOlderRequests**) , et si l'on souhaite intégrer un fader lors du processus de chargement.  
Les requêtes utilisateurs peuvent définir une callback qui sera appelée lorsqu'un requête aura été traitée.

### Gestion des requêtes
Lorsque le **SceneLoaderProxy** est prêt à traiter une requête (pas de transition de fade **ET** le **SceneLoader** est libre), celui-ci dans sa méthode Update va venir récupérer la requête la plus 
ancienne et la traiter. Le traitement de la requête se passe en 4 étapes :
- **Inspection de la requête** : Chaque requête dispose d'une méthode pour s'inspecter en prenant en argument la **SceneColletion**. Le type de retour de l'inspection est 
**SceneLoaderRequestResult** et permet de savoir si l'inspection est un succés, et le message d'erreur dans le cas contraire.
- **Traitement du résultat d'inspection** : Si l'inspection n'est pas un succés, alors la requête est ignorée. On appelle la callback de la requête si celle-ci a été définie
en passant en booléen la valeur de l'inspection.
- **Construction des requêtes** : Si l'inspection est un succés, on va demander à la requête de construire ces InternalSceneRequest afin de pouvoir les envoyer au SceneLoader
- **Envoi des requêtes** : Cette étape peut potentiellement être décalée avec le fader comme on le verra juste en dessous. On envoi les requêtes internes au SceneLoader qui s'occupera de
créer un SceneLoaderProcess et de démarrer le processus.

### Envoi de requêtes au Proxy et demande d'informations
Pour obtenir des informations concernant les scènes chargées, les bundle existants etc... c'est aussi au **SceneLoaderProxy** qu'il faut demander ces informations en utilisant ses 
méthodes agissant comme des getters qui vont chercher les informations dans la **SceneCollection**.

Concernant les envoi de requêtes, il existe un seul point d'entrée via cette méthode :

    public void PostRequest(SceneLoaderRequest loadRequest)
    {
        AttemptClearOlderRequests();
        _requests.Enqueue(loadRequest);
    }

### Fader
Si un fader est assigné dans le SceneLoaderProxy, alors la gestion de l'envoi est décalée. En effet, dans ce cas, le SceneLoaderProxy va venir s'abonner au Fader et suivre ce 
processus concernant l'envoi :
```
isFading = true
X = abonnement à OnFadeInCompleted
FadeIn()

X déclenché
    Envoi les requêtes à SceneLoader
    Y = abonnement à OnFadeOutCompleted    
    FadeOut()

Y déclenché
    isFading = false 
```

Cette gestion permet de s'assurer que l'on ne va pas charger / décharger des scènes (pouvant amener un résultat visuel incorrect) avant d'avoir transitionné sur un visuel noir cachant la scène.

De la même manière, si un fader est assigné, on va venir s'abonner à l'évenement **OnLoaderProcessFinish** du SceneLoaderEvents. Ainsi, lorsque celui-ci sera déclenché
on transitionnera vers un fondu au noir In puis Out. Cette transition moins essentielle permet simplement de s'assurer d'avoir un visuel symétrique et permet de transitionner entre 
la GUI de chargement qui doit normalement être défini et la nouvelle scène courante.

Le SceneLoaderProxy propose 2 événements concernant son fader qui sont :
- **Action OnFadeInCompletedProcessStart** : Averti lorsque le fadeIn pour démarrer un processus vient de terminer
- **Action OnFadeInCompletedProcessFinish** : Averti lorsque le fadeIn de fin de processus vient de terminer


## SceneLoaderGUI
Finissons par le SceneLoaderGUI. Cette classe n'entre pas directement dans le processus de chargement et le SceneLoader fonctionne sans ce dernier. Cependant, le 
SceneLoaderGUI représente l'écran de chargement affiché lors d'un chargement / déchargement de scène.  
Les utilisateurs du SceneManager sont libres de définir comme bon leur semble l'UI afin de transitionner entre les scènes. Ainsi, SceneLoaderGUI est une classe 
abstraite qu'il est nécessaire d'hérité si l'on souhaite s'assurer du bon fonctionnement de l'affichage de l'écran de chargement. Il est cependant possible de ne pas en
hériter si votre implémentation reprends cette ligne de conduite :
        
    private void Start()
    {
        if (Scylla.SceneManagement.SceneManager.Instance.SceneLoaderProxy.Fader != null)
        {
            Scylla.SceneManagement.SceneManager.Instance.SceneLoaderProxy.OnFadeInCompletedProcessStart += OnProxyFadeInCompletedProcessStart;
            Scylla.SceneManagement.SceneManager.Instance.SceneLoaderProxy._onFadeInCompletedProcessFinish += OnProxyFadeInCompletedProcessFinish;
        }
        else
        {
            Scylla.SceneManagement.SceneLoaderEvents.OnLoaderProcessFinish += OnLoaderProcessFinish;
            Scylla.SceneManagement.SceneLoaderEvents.OnLoaderProcessStart += OnLoaderProcessStart;
        }

        Scylla.SceneManagement.SceneLoaderEvents.OnLoaderProcessUpdate += OnLoaderProcessUpdate;
    }
    
    protected virtual void OnProxyFadeInCompletedProcessStart()
    {
        _guiContainer.gameObject.SetActive(true);
    }

    protected virtual void OnProxyFadeInCompletedProcessFinish()
    {
        _guiContainer.gameObject.SetActive(false);
    }
    
    protected virtual void OnLoaderProcessStart()
    {
        _guiContainer.gameObject.SetActive(true);
    }

    protected virtual void OnLoaderProcessFinish()
    {
        _guiContainer.gameObject.SetActive(false);
    }
        
 Encore une fois, il n'est pas nécessaire de définir d'UI écoutant ces événements, cependant si vous faites cela, il n'y aura rien pour cacher l'opération qui peut 
 prendre plusieurs secondes de chargement / déchargement, et le joueur pourra voir la transition de scène, réaliser des actions lorsqu'une première scène est chargée mais 
 pas la deuxième, et pourra avoir l'impression de freeze que peut provoquer un chargement de scène lourd.
