/*
Proje Adı
------------------
Greedy Algoritması ile 8 Puzzle
*/

#include"stdafx.h"
#include<stdio.h>
#include<stdlib.h>

typedef struct anaDugum
{ // puzzelın durumu
    anaDugum* ata;//ata
    int h;  // heureustic değeri (gerçek yerlerine olan uzaklıklarının toplamı)
    int puzzle[9]; // puzzle tablosu
    int bosluk; //boşun konumu
}
anaDugum;

typedef struct hareket
{    // liste, boş karo olası hamle içerir
    int hamleler[4]; // Olası hamle listesi
}
hareket;

//çocuk düğüm oluşturuken kullanıldı
hareket hareketler[9] = { // tablodaki her konumdaki 0'ın hareketleri listesi - 0 eğer 0. indisinde ise 1-3 e gider
	{ 1, 3 },{ 0, 2, 4 },{ 1, 5 },{ 0, 4, 6 },{ 1, 3, 5, 7 },{ 2, 4, 8 },{ 3, 7 },{ 4, 6, 8 },{ 5, 7 }
};

typedef struct {    // düğümlerin listesi

    int dugumSayisi; // listenin düğüm sayısını tutar
anaDugum* elemanlar[57000];
} anaListe;

anaListe fringe; //openList, oluşturulan ancak henüz genişletilmemiş düğümleri saklar
anaListe closedList; //closedList, ziyaret edilen düğümleri saklar
anaListe solutionList; //hedef düğümden,düğüm noktasına düğümleri depolar (çözümün tersi)

/*  bir düğüm için bir alan tahsis etmek*/
anaDugum* alanTahsisEtme(void)
{
    anaDugum* dugum;

    dugum = (anaDugum*)malloc(sizeof(anaDugum));
    dugum->ata = NULL;
    dugum->h = 0;
    return dugum;
}

/* düğüm tarafından ayrılan alanı boşaltın*/
void dugumAlaniSil(anaDugum* dugum)
{
    free(dugum);
    return;
}

/*düğümler listesi listesindeki düğümlerin sayısını döndürür */
int dugumSayisi(anaListe* liste)
{
    return liste->dugumSayisi;
}

/* düğümün düğümler listesi listesinde olup olmadığını kontrol edip ve konumunu listede döndürme ve pos'da saklama */
int dugumKontrol(anaListe* liste, int* puzzle)
{
    int i, j;
    int sayac = 0;

    for (i = 0; i < liste->dugumSayisi; i++)
    {

        sayac = 0; // her ayrı düğümü kontrol ederken sayac sıfırlanıyor.

        for (j = 0; j < 9; j++)
        {
            if (liste->elemanlar[i]->puzzle[j] == puzzle[j])
                sayac++;
            else
                break;

        }
        if (sayac == 9)//listede var ise
        {
            return 1;
        }

    }
    return 0;
}

/* indis konumundaki düğümü listeden sil*/
anaDugum* dugumSil(anaListe* liste, int indis)
{
    anaDugum* node;

    node = liste->elemanlar[indis];
    liste->dugumSayisi--;  //diziler 0 dan başlandığı için 1 eksilterek son eleman indisi bulundu
    liste->elemanlar[indis] = liste->elemanlar[liste->dugumSayisi];//son eleman, düğümden silinecek düğümün yerine atandı.
    liste->elemanlar[liste->dugumSayisi] = NULL;

    return node;
}

/* listede işlenecek en iyi düğümü arayın */
anaDugum* uygunDugumBulma(anaListe* liste)
{
    int i;
    int indis;
    int eniyi = 99;
    anaDugum* dugum;

    int dugumSayisi = liste->dugumSayisi; // fringe eleman sayısı

    for (i = 0; i < dugumSayisi; i++)
        if (liste->elemanlar[i]->h < eniyi)
        {
            indis = i;
            eniyi = liste->elemanlar[indis]->h;
        }
    //en iyi bulundu ve açılacağı için fringeden dugum çıkarıldı.
    dugum = dugumSil(liste, indis);

    return dugum;
}

/* düğümü listeye koy */
void dugumEkle(anaListe* liste, anaDugum* dugum)
{
    int indis = liste->dugumSayisi++;
    liste->elemanlar[indis] = dugum;
}

int sezgiselHesaplama(anaDugum* dugum)
{
    // Manhatten ile gerçek yerlerine olan uzakların toplamı- heuristic hesaplama
    int sezgiselToplam = 0;

    //puzzle 0.indisi için
    if (dugum->puzzle[0] == 1) sezgiselToplam += 0;
    else if (dugum->puzzle[0] == 2) sezgiselToplam += 1;
    else if (dugum->puzzle[0] == 3) sezgiselToplam += 2;
    else if (dugum->puzzle[0] == 4) sezgiselToplam += 1;
    else if (dugum->puzzle[0] == 5) sezgiselToplam += 2;
    else if (dugum->puzzle[0] == 6) sezgiselToplam += 3;
    else if (dugum->puzzle[0] == 7) sezgiselToplam += 2; else if (dugum->puzzle[0] == 8) sezgiselToplam += 3;
    //puzzle 1.indisi için	
    if (dugum->puzzle[1] == 1) sezgiselToplam += 1;
    else if (dugum->puzzle[1] == 2) sezgiselToplam += 0;
    else if (dugum->puzzle[1] == 3) sezgiselToplam += 1;
    else if (dugum->puzzle[1] == 4) sezgiselToplam += 2;
    else if (dugum->puzzle[1] == 5) sezgiselToplam += 1;
    else if (dugum->puzzle[1] == 6) sezgiselToplam += 2;
    else if (dugum->puzzle[1] == 7) sezgiselToplam += 3; else if (dugum->puzzle[1] == 8) sezgiselToplam += 2;
    //puzzle 2.indisi için
    if (dugum->puzzle[2] == 1) sezgiselToplam += 2;
    else if (dugum->puzzle[2] == 2) sezgiselToplam += 1;
    else if (dugum->puzzle[2] == 3) sezgiselToplam += 0;
    else if (dugum->puzzle[2] == 4) sezgiselToplam += 3;
    else if (dugum->puzzle[2] == 5) sezgiselToplam += 2;
    else if (dugum->puzzle[2] == 6) sezgiselToplam += 1;
    else if (dugum->puzzle[2] == 7) sezgiselToplam += 4; else if (dugum->puzzle[2] == 8) sezgiselToplam += 3;
    //puzzle 3.indisi için
    if (dugum->puzzle[3] == 1) sezgiselToplam += 1;
    else if (dugum->puzzle[3] == 2) sezgiselToplam += 2;
    else if (dugum->puzzle[3] == 3) sezgiselToplam += 3;
    else if (dugum->puzzle[3] == 4) sezgiselToplam += 0;
    else if (dugum->puzzle[3] == 5) sezgiselToplam += 1;
    else if (dugum->puzzle[3] == 6) sezgiselToplam += 2;
    else if (dugum->puzzle[3] == 7) sezgiselToplam += 1; else if (dugum->puzzle[3] == 8) sezgiselToplam += 2;
    //puzzle 4.indisi için
    if (dugum->puzzle[4] == 1) sezgiselToplam += 2;
    else if (dugum->puzzle[4] == 2) sezgiselToplam += 1;
    else if (dugum->puzzle[4] == 3) sezgiselToplam += 2;
    else if (dugum->puzzle[4] == 4) sezgiselToplam += 1;
    else if (dugum->puzzle[4] == 5) sezgiselToplam += 0;
    else if (dugum->puzzle[4] == 6) sezgiselToplam += 1;
    else if (dugum->puzzle[4] == 7) sezgiselToplam += 2; else if (dugum->puzzle[4] == 8) sezgiselToplam += 1;
    //puzzle 5.indisi için
    if (dugum->puzzle[5] == 1) sezgiselToplam += 3;
    else if (dugum->puzzle[5] == 2) sezgiselToplam += 2;
    else if (dugum->puzzle[5] == 3) sezgiselToplam += 1;
    else if (dugum->puzzle[5] == 4) sezgiselToplam += 2;
    else if (dugum->puzzle[5] == 5) sezgiselToplam += 1;
    else if (dugum->puzzle[5] == 6) sezgiselToplam += 0;
    else if (dugum->puzzle[5] == 7) sezgiselToplam += 3; else if (dugum->puzzle[5] == 8) sezgiselToplam += 2;
    //puzzle 6.indisi için
    if (dugum->puzzle[6] == 1) sezgiselToplam += 2;
    else if (dugum->puzzle[6] == 2) sezgiselToplam += 3;
    else if (dugum->puzzle[6] == 3) sezgiselToplam += 4;
    else if (dugum->puzzle[6] == 4) sezgiselToplam += 1;
    else if (dugum->puzzle[6] == 5) sezgiselToplam += 2;
    else if (dugum->puzzle[6] == 6) sezgiselToplam += 3;
    else if (dugum->puzzle[6] == 7) sezgiselToplam += 0; else if (dugum->puzzle[6] == 8) sezgiselToplam += 1;
    //puzzle 7.indisi için
    if (dugum->puzzle[7] == 1) sezgiselToplam += 3;
    else if (dugum->puzzle[7] == 2) sezgiselToplam += 2;
    else if (dugum->puzzle[7] == 3) sezgiselToplam += 3;
    else if (dugum->puzzle[7] == 4) sezgiselToplam += 2;
    else if (dugum->puzzle[7] == 5) sezgiselToplam += 1;
    else if (dugum->puzzle[7] == 6) sezgiselToplam += 2;
    else if (dugum->puzzle[7] == 7) sezgiselToplam += 1; else if (dugum->puzzle[7] == 8) sezgiselToplam += 0;
    //puzzle 8.indisi için
    if (dugum->puzzle[8] == 1) sezgiselToplam += 4;
    else if (dugum->puzzle[8] == 2) sezgiselToplam += 3;
    else if (dugum->puzzle[8] == 3) sezgiselToplam += 2;
    else if (dugum->puzzle[8] == 4) sezgiselToplam += 3;
    else if (dugum->puzzle[8] == 5) sezgiselToplam += 2;
    else if (dugum->puzzle[8] == 6) sezgiselToplam += 1;
    else if (dugum->puzzle[8] == 7) sezgiselToplam += 2; else if (dugum->puzzle[8] == 8) sezgiselToplam += 1;


    return sezgiselToplam;
}

/* ilk dugumu aktarma*/
anaDugum* baslangicDugumu(int* puzzle)
{
    int i;
    anaDugum* dugum;

    dugum = alanTahsisEtme();
    for (i = 0; i < 9; i++)
    {
        dugum->puzzle[i] = puzzle[i];

        if (dugum->puzzle[i] == 0)
        {
            dugum->bosluk = i;
        }
    }
    dugum->h = sezgiselHesaplama(dugum);
    return dugum;
}

/*ebeveynden üretilen bir alt düğüm olsun. İndeks, alt düğümün elde edilmesi için hareketin gerçekleştirildiğini belirtmek içindir*/
anaDugum* cocukDugumuOlusturma(anaDugum* dugum, int indis)
{
    anaDugum* cocuk = (anaDugum*)0;
    int cocukSayisi = 0;
    int boslukIndisi;
    int i;
    boslukIndisi = dugum->bosluk;

    if (boslukIndisi == 0 || boslukIndisi == 2 || boslukIndisi == 6 || boslukIndisi == 8)
    {
        cocukSayisi = 2;
    }
    else if (boslukIndisi == 1 || boslukIndisi == 3 || boslukIndisi == 5 || boslukIndisi == 7)
    {
        cocukSayisi = 3;
    }
    else if (boslukIndisi == 4)
    {
        cocukSayisi = 4;
    }
    if (indis < cocukSayisi) //hareketler dizisine göre olası hamle sayısı(dizinin ilk indisi olan uzunluğu aldım)
    {
        int gidilecekindis;

        cocuk = alanTahsisEtme();
        for (i = 0; i < 9; i++)
        {
            cocuk->puzzle[i] = dugum->puzzle[i];
        }
        cocuk->bosluk = dugum->bosluk;
        gidilecekindis = hareketler[boslukIndisi].hamleler[indis];//gidilicekindise,hareketler structın boslukindisinci elemanının hamleler sayılarından indisinci sayısını atadık
        cocuk->puzzle[cocuk->bosluk] = cocuk->puzzle[gidilecekindis];//gidilicek indis değeri,boşluğa atandı.
        cocuk->puzzle[gidilecekindis] = 0;//yer değiştirme
        cocuk->bosluk = gidilecekindis; //yer değiştirme

    }

    return cocuk;
}

/*hedefi buldukan sonra sondan başlayarak ataları listeye koyarak çözümü bulma*/
void cozumListesi(anaListe* cozumListesi, anaDugum* hedef)
{
    anaDugum* yedekList[10000];
    int i = 0;
    while (hedef != NULL)
    {
        yedekList[i] = hedef;//en son puzzle yedek listenin 0.indisine atandı.
        hedef = hedef->ata;
        i++;
    }

    for (int x = i; i < 0; x--)
    {
        for (int y = 0; y < i; y++)
        {
            cozumListesi->elemanlar[y] = yedekList[x];
        }

    }

}

/* çözüm bulmak için greedy algoritması */
void greedyAlgoritmasi(void)
{
    anaDugum* kokDugum, *cocuk;
    int i;

    while (dugumSayisi(&fringe) < 999999)
    { //hedefi bulana kadar dönsün
        kokDugum = uygunDugumBulma(&fringe); // listede en iyi düğüm bulunur
        dugumEkle(&closedList, kokDugum); // düğümü, ziyaret edilen düğümlerin listesine koy
        if (kokDugum->h == 0)
        { // Mevcut düğüm hedef düğümse çözümü alın ve algoritmayı durdurun
            cozumListesi(&solutionList, kokDugum);
            return;
        }
        else
        {

            for (i = 0; i < 4; i++)
            {   // geçerli düğümün alt düğümlerini oluştur
                cocuk = cocukDugumuOlusturma(kokDugum, i);
                if (cocuk != NULL)
                {
                    if (dugumKontrol(&closedList, cocuk->puzzle) != 1)
                    { // düğüm,açılan düğümler listesinde yer almıyorsa
                        cocuk->h = sezgiselHesaplama(cocuk);
                        cocuk->ata = kokDugum;
                        dugumEkle(&fringe, cocuk);
                    }
                }
            }
        }
    }

    return;
}

/*çözüm yazdırma işlevleri */
void dugumYazdirma(anaDugum* dugum)
{
    int i;

    for (i = 0; i < 9; i++)
    {
        if (i == 0 || i == 3 || i == 6)
        {
            printf("         ");
        }
        printf("%d  ", dugum->puzzle[i]);
        if ((i + 1) % 3 == 0)
            printf("\n");
    }
    printf("\n*************************\n");
}

void cozumListesiYazdirma(anaListe* liste)
{
    int i;
    printf("***** Puzzle Cozumu ***** \n \n");
    for (i = 0; i < liste->dugumSayisi; i++)
        dugumYazdirma(liste->elemanlar[i]);
}

int main()
{
    anaDugum* dugum;
    //int puzzle[9] = { 6,2,3,1,4,5,8,7,0 };
    /*{ 0,2,3,1,4,5,6,7,8 };*/
    int puzzle[9];
    for (int i = 0; i < 9; i++)
    {
        printf_s("Puzzle oyununun %d.sayisini giriniz: ", i + 1);
        scanf_s("%d", &puzzle[i]);
    }
    dugum = baslangicDugumu(puzzle); // ilk düğüm
    printf("\n\n******* ilk Puzzle *******\n\n");
    dugumYazdirma(dugum);
    dugumEkle(&fringe, dugum);
    greedyAlgoritmasi();
    cozumListesiYazdirma(&solutionList);
    getchar();
    getchar();
    return 0;
}