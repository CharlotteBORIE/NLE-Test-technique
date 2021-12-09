using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class CreateListe : MonoBehaviour
{
    //Constantes
    private string[] nom_espèces={"Salix", "Gaultherian","Vhalrax"," Ga'Borgah","Tilia"};
    private int marge=10; //10% de similitudes entre 2 listes consécutives

    
    //Variables
    private int nb_joueurs;
    private List<string> joueurs;
    private Dictionary<string,string> joueur_espece;
    private Dictionary<string,int> inventaire_espece;

    private List<GameObject> objet_joueurs;

    //Unity Objects
    public GameObject joueur_prefab;
    public Scrollbar scroll;


    // Start is called before the first frame update
    void Start()
    {
        nb_joueurs=0;
        joueurs=new List<string>();
        joueur_espece=new Dictionary<string, string>();
        inventaire_espece=new Dictionary<string, int>();
        objet_joueurs=new List<GameObject>();
        Delete();
        
    }

    public void nouvelleListe()
    {
        int nouveau_nb_joueurs=20+Random.Range(0,31);

        //create list of players
        List<string> nouveau_joueurs=new List<string>();

        for (int i = 0; i < nouveau_nb_joueurs; i++) 
        {
            nouveau_joueurs.Add(i.ToString());
        }


        //create player/race dictonnary
        Dictionary<string, string> nouveau_joueur_espece=new Dictionary<string, string>();
        Dictionary<string, int> nouveau_inventaire_espece=new Dictionary<string, int>();

        for (int i = 0; i < nouveau_nb_joueurs; i++) 
        {
            int choix_espece=Random.Range(0,5);
            string espece=nom_espèces[choix_espece];
            nouveau_joueur_espece.Add(nouveau_joueurs[i],espece);

            if(nouveau_inventaire_espece.ContainsKey(espece)){
                nouveau_inventaire_espece[espece]++;
            }
            else{
                nouveau_inventaire_espece[espece]=1;
            }
        }

        //Compare to the previous liste
        int similarity_marge=(int)nouveau_nb_joueurs*marge/100;

        int nombre_similaire=0;
        for (int i = 0; i < Mathf.Min(nouveau_nb_joueurs,nb_joueurs); i++)
        {
            string joueur=nouveau_joueurs[i];
            if(joueur_espece.ContainsKey(joueur)){
                if(nouveau_joueur_espece[joueur]==joueur_espece[joueur]){
                    nombre_similaire++;
                }
            }
        }

        if(nombre_similaire>similarity_marge){
            nouvelleListe();
        }
        else{
            nb_joueurs=nouveau_nb_joueurs;
            joueurs=nouveau_joueurs;
            joueur_espece=nouveau_joueur_espece;
            inventaire_espece=nouveau_inventaire_espece;
        }
        //Displaying
        Display();
    }

    public void nouveauJoueur(){
        nb_joueurs++;
        joueurs.Insert(0,nb_joueurs.ToString()); //to put player on top

        //select the species of this player
        foreach(string espece in inventaire_espece.Keys){
            Debug.Log(espece+" : "+inventaire_espece[espece]);
        }
        Debug.Log("Proportions");

        //find min
        int nombre_espece_minoritaire=nb_joueurs;
        foreach(string espece in inventaire_espece.Keys){
            if(inventaire_espece[espece]< nombre_espece_minoritaire){
                nombre_espece_minoritaire=inventaire_espece[espece];
            }
        }

        //Define proportionnalities
        List<string> proportion_espece=new List<string>();
        List<string> minimum_especes=new List<string>();
        //calculate the chances of each  species
        foreach(string espece in inventaire_espece.Keys.ToList()){ 
                int proportion=Mathf.Max(0,(2*nombre_espece_minoritaire-inventaire_espece[espece]));
                //add to proportion_espece
                for(int i=0;i<proportion;i++){
                    proportion_espece.Add(espece);
                }
            Debug.Log(espece+" : "+proportion);
            if(proportion==nombre_espece_minoritaire){//we have a minimum specie
                minimum_especes.Add(espece);
            }
        }
        //Add the minimum species neede to get the right proportions for the top ones 
        int index=0;
        while(proportion_espece.Count<5*nombre_espece_minoritaire){
            proportion_espece.Add(minimum_especes[index%minimum_especes.Count]);
        }

        int randomIndex = Random.Range(0, proportion_espece.Count);
        string espece_choisi=proportion_espece[randomIndex];

        //create player
        joueur_espece[nb_joueurs.ToString()]=espece_choisi;
        if(inventaire_espece.ContainsKey(espece_choisi)){
            inventaire_espece[espece_choisi]++;
        }
        else{
            inventaire_espece[espece_choisi]=1;
        }
        Display();
    }


    void Display(){
        Delete();
        objet_joueurs.Clear();
        for (int i = 0; i < nb_joueurs; i++)
        {
            //Create Object
           GameObject objet;
           if(i==0){
               objet=joueur_prefab;
           }
           else{
                objet=Instantiate(joueur_prefab,joueur_prefab.transform.position+new Vector3(0,-i,0),new Quaternion (0,0,0,0),joueur_prefab.transform.parent);
                objet_joueurs.Add(objet);
           }

            //Renamming
            objet.name="Joueur " + joueurs[i];
            objet.GetComponent<TextMeshProUGUI>().text=joueurs[i];

            objet.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text=joueur_espece[joueurs[i]];

        }
        scroll.value=1;
    }

    void Delete(){
        //Clear the first one
        joueur_prefab.GetComponent<TextMeshProUGUI>().text=" ";
        joueur_prefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text=" ";

        //destroy the rest
        foreach (GameObject item in objet_joueurs)
        {
            Destroy(item);
        }
    }
}
