
;Autor: Micha³ Jankowski
; Dzieñ: 13.12.2019r.
; Przedmiot: Jêzyki Asemblerowe
; Temat: Efekt Sepii

 ; Changelog:
 ; v. 0.1  14/09/19r. Dodanie pliku asm 64 bitowym z prostym sprawdzeniem czy argumenty s¹ przesy³ane do pliku dll, pomyœlna kompilacja procedury
 ;
 ; v. 0.2  21/09/19r. Wstawienie wstêpnych komentarzy do kodu w celu jego pó¿niejszego, lepszego zrozumienia. Analiza przesy³ania danych przez rejestry.
 ;
 ; v. 0.2  13/10/19r. Rozpoczêcie implemenratji odcienie szaroœci w postaci procedury Sepia proc. Nieudane wyjœcie z procedury. 
 ; przesy³anie parametrów przez odpowiednie rejestry.
 ;
 ; v. 0.4  18/10/19r. Funkcjonuj¹ca implementacja odcienie szaroœci w procedurze Sepia z wykorzystaniem 
 ; rejestrów 128 bitowych oraz pêtli average_loop. Jednoczesne przetwarzanie 2 pikseli. 
 ; dodanie dyrektywy .data w celu przechowywania sta³ych programu niezbêdnych do przesuwania sk³adowych pikseli
 ;
 ; v. 0.5  21/10/19r. Dodanie kolejnej pêtli toneloop w celu dokoñczenia algorytmu poprzez koloryzacje odpowiednich sk³adowych obrazu
 ;
 ; v. 0.6  25/10/19r. Testowanie danych wejœciowych algorytmu dla ró¿nych obrazów i rozmiarów m.in. obrazów o rozmiarze 4k x 4k pikseli
 ;
 ; v. 0.7  27/10/19r. Napotkanie problemu z niepoprawnym zliczaniem rozmiaru tablicy dla wieluw¹tków. Wprowadzenie równie¿ przedzia³ów 
 ; dla przetwarzanych czêœci obrazu, w celu unikniêcia hazardu danych.
 ;
 ; v. 0.8  1/11/19r.  Naprawa b³êdu z poprzedniej wersji. Prze³o¿enie procedury na rejestry 256 bitowe ymm w celu wykorzystania mo¿liwoœci procesora
 ; dodanie komentarzy do reszty niepokomentowanego kodu. Poprawa przejrzystoœci kodu poprzez jego odpowiednie oddzielenie
 ;
 ; v. 0.9  18/11/19r. Poprawa drobnych usterek w postaci z³ej wartoœci sta³ej w masce color_R_mask. Dodanie procedur
 ; DetectFeatureEDX oraz DetectFeatureECX w celu sprawdzenia ob³usigwanych procedur przez procesor oraz posiadanych rejestrów.
 ; dodanie komentarzy w asm poprzez dodanie opisu parrametrów wej. i wyj. zwracanych parametrów oraz celu wykonywania danych operacji w asemblerze
 ; poprawa przejrzystoœci kodu i odpowienie odstêpy komentarzy
 
 ; v 1.0  13/12/19r.
 ;
 ;
 ;
 ;

 ; parametry wejœciowe dla funkcji wysokiego poziomou w C#
 ; Sepia(ptr, start, stop, toneValue)
 ; opis:
 ; ptr -> wskaŸnik do tablicy bajtów obrazu typu byte
 ; start -> pocz¹tek przedzia³u dla tablicy obrazu, dla której dany w¹tek wykonuje obliczenia typu int
 ; stop -> koniec przedzia³u dla tablicy obrazu, dla której dany w¹tek wykonuje obliczenia typu int
 ; toneValue -> wartoœæ wype³nienia sepii typu int

												 ; Deklaracja sta³ych wykorzystywanych w celu maskowania odpowiednich wartoœci sk³dowych piksela R,G,B 
.data
divider_value dq   003000000030000h				 ; divider_value -> sta³a wartoœæ 3 zapisania w postaci maski w celu wykonania dzielenia wektorowego
alpha_mask dq      0ff000000ff000000h			 ; alpha_mask ->   maska kana³u przezroczystoœci dla ka¿dego piksela
color_R_mask dq    00ff000000ff0000h			 ; color_R_mask -> maska koloru czerwonego dla ka¿dego piksela
.code

												 ; procedura odpowiedzialna za przetwarzanie kolorowego obrazu do efektu sepii
												 ;----------------------------------------------------------------------------
												 ; parametry wejœciowe: przekazywane przez rejestry
												 ; rcx -> adres obrazka
												 ; rdx -> start przedzia³u tablicy
												 ; r8 -> koniec przedzia³u tablicy
												 ; r9 -> wspó³czynnik wype³nienia
												 ;-----------------------------------------------------------------------------
												 ; parametry wyjœciowe:
												 ; void

Sepia proc									     ; poczatek procedury Sepia
								           
mov r10, rcx									 ; ³adownie adresu obrazka do rcx, aby go zapamiêtaæ

mov r11, rdx									 ; ³adowanie poczatku tablicy do r11 na póŸniejsze przetwarzanie tablicy

mov rdi, r10								     ; przenesienie adresu tablicy pikseli obrazka do rejestru rdi (rejestr indeksowy), aby obliczyæ indeks od którego bêdziemy zliczaæ piksele

add rdi, rdx								     ; dodanie przesuniecia zwiazanego z podzialem na watki do rejestru rdi, w rejestrze rdi aktualnie znajduje siê adres tablicy pikseli obrazka

mov rcx, r8								         ; za³adowanie do rcx  koniec przedzia³u tablicy

mov r12, r9										 ; za³adowanie wartoœci wype³nienia  sepii do rejrestru r12
 
sub rcx, rdx								     ; za³adowanie iloœci bitów do przetworzenia, czyli koniec przedzia³u  - poczatek przedzia³u  do rejstru rcx 

movlps xmm0, color_R_mask						 ; ³adowanie sta³ej maski koloru czerwonego R do rejestru xmm0, w celu po¿niejszego zapisywania  danych sk³adowych piksela

vinsertf128 ymm0,ymm0, xmm0, 1					 ; ³adowanie sta³ej maski koloru  czerwonego R do  dolnej czêsci rejestru ymm0 z wykorzystaniem rejestru xmm0, aby wykonywaæ operacje wektorowe na ca³ym rejestrze ymm0

movlps xmm1, divider_value					   	 ; ³adowanie sta³ej maski liczby "3" (divider_value) dla wartoœci sk³adowych piksela  do rejestru xmm1, aby wykonaæ dzielenie przez sumê sk³adowych danego piksela

vinsertf128 ymm1,ymm1, xmm1,1					 ; ³adowanie sta³ej maski liczby "3" (divider_value)  do dolnej czêsci rejestru ymm1 z wykorzystaniem rejestru xmm1, aby wykonywaæ operacje wektorowe na ca³ym rejestrze ymm1

movlps xmm2, alpha_mask							 ; ³adowanie sta³ej maski przezroczystoœci dla kana³u alpha do rejestru xmm2, aby zapamiêtaæ kana³ przezroczystoœci dla piksela

vinsertf128 ymm2, ymm2, xmm2, 1				     ; ³adowanie sta³ej maski przezroczystoœci do  dolnej czêsci rejestru ymm2 z wykorzystaniem rejestru xmm2, aby  zapamiêtaæ kana³ przezroczystoœci dla 4 pikseli

vpshufd ymm0, ymm0, 00h						     ; ustawianie maski koloru czerwonego w ca³ym obszarze rejestru ymm0, pocz¹wszy od pocz¹tku rejestru ymm0.
                                                 ; w celu wykonania maskowania dla ca³ego rejestru z uwzglêdnieniem odpowiedniego przesuniêcia na pocz¹tek wartoœci
												
vpshufd ymm1, ymm1, 00h						     ; ustawianie dzielnika w ca³ym obszarze rejestru ymm1, pocz¹wszy od pocz¹tku rejestru ymm1
											     ; w celu wykonania maskowania dla ca³ego rejestru z uwzglêdnieniem odpowiedniego przesuniêcia na pocz¹tek wartoœci	

vpshufd ymm2, ymm2, 00h						     ; ustawianie maski kana³u przezroczystoœci w ca³ym obszarze rejestru ymm2, pocz¹wszy od pocz¹tku rejestru ymm2
											     ; w celu wykonania maskowania dla ca³ego rejestru z uwzglêdnieniem odpowiedniego przesuniêcia na pocz¹tek wartoœci

vcvtdq2ps ymm3, ymm1					         ; zamiana wartoœci dzielnika 3 (divider_value) z typu int na float w celu wykonania dzielenia wektorowego przez liczby zmiennoprzecinkowe i zapamiêtanie w rejestrze ymm3

average_loop:									 ; start pêtli wykonuj¹cej odcienie szaroœci algorytmu. Czêœæ pierwsza wykonywania ca³ego algorytmu sepii

movdqu xmm10, [rdi]								 ; pobranie 2 pikseli z tablicy bajtów obrazka  z rejestru rdi do xmm10, aby wykonaæ na nich operacje niezbêdne do wykonania algorytmu sepii 

add rdi, 16									     ; przesuniecie rejestru indeksowego o 16 pozycji do przodu, w celu pobrania nowych wartoœci z rejestru rdi

sub rcx, 16										 ; przesuniecie rejestru zliczaczj¹vego o 16 pozycji do ty³u, aby nie wyjœæ poza  zakres tablicy 

movdqu xmm9, [rdi]								 ;  pobranie kolejnych 2 pikseli z rejestru rdi do xmm9, aby wykonaæ na nich kolejne operacje niezbêdne do wykonania algorytmu sepii


vinsertf128 ymm4,ymm4,xmm10,0					 ; przesuniecie 2 pikseli do górnej czêœci rejestru ymm4, aby póŸniej wykonaæ na nich operacje wektorowe SIMD na 4 pikselach

vinsertf128 ymm4,ymm4,xmm9,1					 ; przsuniecie 2 piskeli do dolnej czesci rejestru ymm4, aby pózniej wykonaæ operacje wektorowe SIMD na 4 pikselach

												 ; przetrzymywanie aktualnie 4 pikseli jednoczeœnie w rejestrze ymm4
          
vmovaps ymm6, ymm4								 ; zapamietanie skladowych alpha 4 kolejnych pikseli w rejestrze ymm6, aby odpowiednio póŸniej przekazaæ wartoœci kana³u przezroczystoœci do przetworzonych pikseli


vpand ymm6, ymm6, ymm2							 ; maskowanie kana³u przezroczystoæi 4 kolejnych pikseli w rejestrze ymm6, aby otrzymaæ jedynie wartoœci skladówych piksela R,G,B

vmovaps ymm7,ymm4							     ; przepisanie 4 kolejnych pikseli bez kana³u przezroczystoœci do rejestru ymm7 w celu wyci¹gania z podanego rejestru odpowiednich sk³adowych piksela 

vpslldq ymm7,  ymm7 , 1							 ; logiczne przesuniêcie rejestru ymm7 o 2 wartoœci w zapisie szesnastkowym w lewo, którym jest zadeklarowana wczeœniej sta³¹ maska koloru R, abyy otrzymaæ maskê koloru G
												 ; Wykorzystywana do maskowania kolorów zielonych dla danego piksela i jednoczeœnie przetrzymywanie tych wartoœci w podanej pozycji

vmovaps ymm8, ymm7								 ; zapamiêtanie  rejstru ymm7 do ymm8, aby przechowywaæ sta³¹ maskê koloru zielonego 


vpslldq  ymm8,ymm8, 1							 ; logiczne przesuniêcie rejestru ymm8 o 2 wartoœci w zapisie szesnastkowym w lewo, którym jest zadeklarowana wczeœniej sta³¹ maska koloru G, abyy otrzymaæ maskê koloru B
												 ; Wykorzystywana do maskowania kolorów niebieskich dla danego piksela i jednoczeœnie przetrzymywanie tych wartoœci w podanej pozycji

												 
vpand ymm4, ymm4, ymm0						     ; zamaskowanie sk³adowych piksela A, G, B (A - kana³ alhpa) rejestru ymm4 z wykorzystaniem maski kolory czerwonego z rejestru ymm0.W ten sposób uzyskujemy same kolory czerwone R pikseli
												 
vpand ymm7,ymm7,ymm0							 ; zamaskowanie sk³adowych piksela A, R, B rejestru ymm7 z wykorzystaniem maski kolory czerwonego z rejestru ymm0. Uzyskujemy same kolory zielony G pikseli
 
vpand ymm8, ymm8, ymm0						     ; zamaskowanie sk³adowych piksela A, R, B rejestru ymm8 z wykorzystaniem maski kolory czerwonego z rejestru ymm0. Uzyskujemy same kolory zielony B piksei
												 
vpaddd ymm5, ymm4, ymm7		 					 ; zsumowanie rejestrów ymm4 i ymm7 do rejestru ymm5. Uzyskujemy zsumowane kolry czerwone R i zielone G ( R+G). W celu wykonania œredniej arytmetycznej sk³adowych piksela.

vpaddd ymm5,ymm5, ymm8							 ; zsumowanie rejestrów ymm5 i ymm8 do rejestru ymm5. Uzyskujemy sumê kolory czerwonego i zielonego (R+G) oraz koloru niebieskiego (R+G+B). Mo¿na dziêki temu wykonaæ obliczyæ œredni¹ arytemetyczn¹ piksela.


												
vcvtdq2ps ymm7, ymm5						     ; zamiana wartoœci int rejestru ymm6 na float do rejestru ymm7 w celu  wykonania dzielenia wektorowego  dzielenie wartoœci 3 kolejnych skladowcyh pikseli przez wartosc sta³ej maski dzielnika 3 (divider_value), 

												
vdivps ymm7, ymm7, ymm3							 ; wykonanie dzielenia wektorowego w celu otrzymania œredniej arytmetycznej (R+G+B) / 3, dzielenie wektorowe rejestru  ymm7 przez rejestr sta³ego dzienika 3 (divider_value) do rejestru ymm7
												 
											     
vcvtps2dq ymm5,ymm7								 ; zamiana wartoœci float rejestru ymm7 na int do rejstru ymm5, aby póŸniej móc zapisywaæ wartoœci do tablicy bajtów

vmovaps ymm7, ymm5								 ; przniesienie rejstru ymm5 do rejestru ymm7, aby zapamiêtaæ wartoœci œredniej arytmetycznej pikseli do póŸniejszego przetwarzania

vpslldq ymm5,ymm5, 1							 ; logiczne przesuniêcie rejestru ymm5 ( aktualne obliczone œrednie arytmetyczne pikseli) o 2 wartoœci szesnastkowe w celu wpisania wartoœci X na odpowiedni¹ pozycjê, gdzie X = (R + B + G ) / 3 do rejestr ymm5 
											     ; reprezentacja wartoœci w rejestrze ymm5: 0,X,X,X

vpor ymm7,ymm7,ymm5							     ; logiczna operacja OR na rejestrze ymm5 i ymm7 do rejestru ymm7, w celu ustalenia pozycji piksela na odpowiednim bicie
                                                 ; Wykonywana w celu ustalenia 2 pierwszych pozycji dla  pikseli

vpslldq ymm5,ymm5, 1					         ; logiczne przesuniêcie rejestru ymm5 o 2 wartoœci szesnastkowo w celu wpisania wartoœci X wyjaœnionych powy¿ej na odpowiedni¹ pozycjê
												
										 

												

vpor ymm7, ymm7, ymm5							 ; logiczna operacja OR na rejestrze ymm5 i ymm7 do rejestru ymm7, w celu ustalenia pozycji piksela na odpowiednim bicie
												 ; Wykonywana w celu ustalenia 2 ostatnich pozycji dla  pikseli				
						
													
												 ; przepisanie watosci kanalu alpha				
vpor ymm7, ymm7, ymm6							 ; logiczny OR na rejestrze ymm7 i ymm5 w celu przepisanie do rejestru ymm7 wartoœci kana³u alpha z rejestru ymm6 na odpowiedni¹ pozycjê dla pikseli

										

vextractf128 xmm10,ymm7,1						 ; przepisanie dolnej czêœci rejestru ymm7 do rejestru xmm10 w celu wpisania wynikowych wartoœci do tablicy ( 2 piksele)

movdqu [rdi], xmm10								 ; przesuniêcie wartoœci wynikowych pikseli z rejestru xmm10 do rejestru indeksowego rdi, gdzie s¹ one zapisywane do tablicy 


vextractf128 xmm11, ymm7,0						 ; przepisanie górnej czêœci rejestru ymm7 do rejestru xmm11 w celu wpisania wynikowych wartoœci do tablicy ( 2 piksele)

sub rdi, 16										 ; przesuniecie rejestru indeksowego o 16 pozycji do ty³u, w  celu umieszczenia pikseli do elementów tablicy obrazka znajduj¹cej siê o 16 elementów wczeœniej

movdqu [rdi], xmm11								 ; przesuniêcie wartoœci wynikowych pikseli z rejestru xmm11 do rejestru indeksowego rdi, gdzie s¹ one zapisywane do obrazka

add rdi, 32										 ; przesuniêcie rdi o 32, aby znajdowaæ siê na w³aœciwej pozycji obrazka w trakcie przetwarzania pikseli

sub rcx, 16										 ; odjêcie od indeksu zliczaj¹cejgo 16, aby nie wyjœæ poza zakres tablicy

cmp rcx, 0										 ; sprawdzenie czy przekroczono rozmiar tablicy i zakoñczono zliczanie

jle prepare										 ; je¿eli  zosta³ przekoroczny rozmiar tablicy przygotowujemy do wype³nienia obrazka efektem sepii

jmp average_loop								 ; bezwarunkowy skok do pêtli licz¹cej œredni¹ arytmetyczn¹, czyli powrót do wykonywania pierwszej czêœci algorytmu

prepare:									     ; pêtla przygotowuj¹cac do wype³nienia obrazka nadaj¹cy efekt sepii. Przypisywanie wartoœci pocz¹tkowych dla danych rejestrów

mov rdi, r10									 ; przepisanie do rdi pocz¹tku adresu obrazka

add rdi, r11									 ; ³adowanie pocz¹tku  zliczania elementów tablicy zwi¹zanego z podzia³em na w¹tki poprzez dodanie do rejestru rdi wartoœci pocz¹tkowej tablicy rejestru r11

mov rcx, r8										 ; za³adowanie do rcx iloœci bajtów do przetworzenia

sub rcx, r11									 ; odjêcie przesuniêcia (offsetu) zwi¹zanego z podzia³em na w¹tki z rejestru rcx poprzez odjêcie koñca przedzia³u zlicania z rejestru r11


mov r12, 255									 ; za³adowanie do rejestru r12 wartoœci 255, aby sprawdzaæ czy nie przekroczono maksymalnej dozwolonej wartoœci piksela

mov r13, r12								     ; zapamiêtanie wartoœci rejestru r12 do rejestru r13, w celu póŸniejszego wykorzystania rejestru r12 ( wartoœci 255)

              					     
sub r13, r9									     ; odjêcie od rejestru 13 wartoœci spod rejestru r9. 255 - x, gdzie x to wartoœæ zadanej przez u¿ytkownika efektu wype³nienia. Wykorzystywana do sprawdzenia czy wartoœæ wype³nienia sepii nie przekracza 255

sub r13, r9			    						 ; kolejne odjêcie do rejestru r13 wartoœci spod rejestru r9 wynikaj¹cego z algorytmu

cmp rax, r13	                                 ; porównanie rejestru rcx z r13 czy wartoœæ wype³nienia nie przekracza maksymalnej wartoœci 255

ja max_red_t								     ; skok w przypadku przekroczenia maskymalnej wartoœci 255


add rax,r9										 ; je¿eli nie przekroczono mmaksymalnej wartoœci dodajemy wartoœæ wype³nienia do akumulatora raxd la czerwonej sk³adowej  piksela

add rax,r9										 ; drugi raz dodajemy do akumulatora wartoœæ wype³nienia, poniewa¿ wynika to z algorytmu

 
jmp continue_red	   							 ; skok bezwarunkowy do etykiety kontynuj¹cej wype³nienia pikseli, poprzez pobranie kolejnego piksela

max_red_t:				    					 ; etykieta w przypadku przekroczenia maskymalnej wartoœci 255

mov rax, r12									 ; za³adowanie do  sk³adowej czerwonej  piksela wartoœci maksymalnej 255, poniewa¿ wspó³czynnik wype³nienia przekroczy³ maksymaln¹ wartoœæ 255


toneloop:									     ; pêtla wykonuj¹ca  wype³nienia piksela dla obrazka efektem Sepii i jego kontunuacja z pocz¹tkowej pêtli

mov al, [rdi+1]								     ; przesuniêcie do akumulatora al wartoœci obrazka

mov r13, r12								     ; przesuniêcie do r13 wartoœci 255, w celu sprawdzenia czy nie przekroczono maskymalnej dopuszczalnej wartoœci

sub r13,r9										 ; odjêcie od rejestru 13 wartoœci spod rejestru r9. 255 - x, gdzie x to wartoœæ zadanej przez u¿ytkownika efektu wype³nienia

cmp rax, r13								     ; porówanie z akumulatorem rax czy przekroczono maksymaln¹ wartoœæ 255

ja max_green									 ; skok w przypadku przekroczenia maskymalnej wartoœci 255

add rax, r9										 ; je¿eli nie przekroczono mmaksymalnej wartoœci dodajemy wartoœæ wype³nienia do akumulatora rax dla zielonej sk³adwoej  piksela

jmp continue_green								 ; skok do etykiety kontynuj¹cej wype³nianie sk³adowej zielonej piksela

max_green:										 ; etykieta wpisuj¹ca maksymaln¹ wartoœæ 255 dla koloru zielonego, poniewa¿ przekroczy³ maksymaln¹ dopuszczaln¹ wartoœæ

mov rax,r12										 ; wpisujemy do  akumulatora rax wartoœæ 255  z rejestru r12 dla sk³adowje zielonej piksela, wynika to z algorytmu

continue_green:									 ; etykieta do kontynowania efektu wype³niania i wsadzenia wartoœci obliczonego wype³nienia do tablicy

mov [rdi+1],al									 ; przesuniêcia wartoœci akumulatora al do adresu obrazka, gdzie akumulator zawiera obliczon¹ wartoœæ wype³nienia


mov al,[rdi+2]				    				 ; pobranie kolejnej wartoœci piksela do akumulatora al, aby wykonaæ obliczanie algorytmu dla dalszych pikseli

mov r13, r12								     ; przesuniêcie do r13 wartoœci 255, aby sprawdziæ czy nie przekorczono maskymalnej wartoœci

sub r13, r9										 ; odjêcie od rejestru 13 wartoœci spod rejestru r9. 255 - x, gdzie x to wartoœæ zadanej przez u¿ytkownika efektu wype³nienia

sub r13, r9			    					     ; kolejne odjêcie do rejestru r13 wartoœci spod rejestru r9 wynikaj¹cego z algorytmu

cmp rax, r13									 ; porówanie z akumulatorem rax czy przekroczono maksymaln¹ wartoœæ 255

ja max_red										 ; skok w przypadku przekroczenia maskymalnej wartoœci 255


add rax,r9										 ; je¿eli nie przekroczono mmaksymalnej wartoœci dodajemy wartoœæ wype³nienia do akumulatora rax wartoœæ sk³adowej czerwonej piksela

add rax,r9								         ; drugi raz dodajemy do akumulatora wartoœæ wype³nienia


jmp continue_red	   						     ; skok bezwarunkowy do etykiety kontynuj¹cej wype³nienia sk³adowej czrwonej piksela

max_red:				    					 ; etykieta w przypadku przekroczenia maskymalnej wartoœci 255

mov rax, r12								     ; wpisujemy do  akumulatora rax wartoœæ 255  z rejestru r12 dla sk³adowej czerownej piksela, poniewa¿ przekroczono maksymaln¹ dopuszczaln¹ wartoœæ

continue_red:			   						 ; etykieta do kontynowania efektu wype³niania i wsadzenia wartoœci obliczonego wype³nienia do tablicy

mov [rdi+2], al								     ; przesuniêcia wartoœci akumulatora al do adresu obrazka, gdzie akumulator zawiera obliczon¹ wartoœæ wype³nienia

add rdi, 4									     ; przesuniêcie o 4 bity ( 1 piksel) do przodu wartoœci  rejestru indeksowego, gdzie znajdujê siê adres obrazka

sub rcx, 4									     ; odjêcie do wartoœci rejestru zliczaj¹cego 4 ( 1 piksel), aby nie wyjœæ poza zareks tablicy

cmp rcx, 0									     ; sprawdzenie czy zakoñczono zliczanie

jle koniec									     ; je¿eli wartoœæ poni¿ej zera albo równa zeru skocz do etykiety koniec, zakoñczono zliczanie dla drugiej czêœci algorytmu

jmp toneloop									 ; skok bezwarunkowy do pêtli zewnêtrznej wykonuj¹cej g³ówny proces wype³niania obrazka, poniewa¿ nie zakoñczono przekszta³cania obrazka 
  
koniec:											 ; etykieta, w której koñczymy wykonywanie naszego przetwarzania obrazka 

ret											     ; powrót z  g³ównej procedury Sepia

Sepia endp										 ; koniec procedury wykonywania Sepii


 ; procedura do sprawdzania osb³ugi rozkazów  MMX
 ; inicjalizacja procedury w jêzyku wysokiego poziomu:  public static extern bool DetectFeatureEDX(int check);
 ; parametry wejœciowe: int  -> wartoœæ szukanej funkcjonalnoœci procesora
 ; parametry wyjœciowe: bool -> 1 dla ob³ugiwanej funkcjonalnoœci, a 0 to jej brak
DetectFeatureEDX proc						  


push rbx									    ; zapis wyowa³ania procedury do rbx, aby jej nie utraciæ

mov r8b, cl										; zapisanie poszukiwanej funkcjonalnoœci do r8b


mov eax, 1									    ; nazwa obs³ugiwanej  funkcji przez proceor zapisywana  do rejestru eax

cpuid											; wywo³aj CPUID, aby ustawiæ odpowiedni bit w rejectrze edx poszukiwanej funkcjonalnoœci

mov eax, edx								    ; przenieœ  ustawiony bit odpowiedzi  z edx to eax, aby go zapamiêtaæ

mov cl, r8b									    ; przenieœ  bit z r8b do cl, aby pó¿niej wykonaæ na cl logiczne przesuniêcie w celu ustawienia rejestru edx na odpowiedni¹ pozycje


shr eax, cl									    ; przesuñ bit odpowiedzi na 0. pozycje rejestru eax 

and eax, 1									    ; zamaskuj pozosta³e bity odpowiedzi na 0, aby wyci¹gn¹æ tylko oczekiwan¹ przez nas infromacje o funkcjonalnoœæi procesora do akumulatora

pop rbx										    ; zwróæ wywo³anie procedury do rbx
												; zwróc 1 lub 0 do akumulatora al, gdzie 1 to obs³uga danej funkcjonalnoœci, a 0 to jej brak

ret												; koniec obs³ugi procedury  sprawdzania osb³ugi rozkazów  MMX

DetectFeatureEDX endp           

; procedura do sprawdzania osb³ugi rozkazów  MMX
; inicjalizacja procedury w jêzyku wysokiego poziomu:  public static extern bool DetectFeatureECX(int check);
; parametry wejœciowe: int  -> wartoœæ szukanej funkcjonalnoœci procesora
; parametry wyjœciowe: bool -> 1 dla ob³ugiwanej funkcjonalnoœci, a 0 to jej brak
 
DetectFeatureECX proc			  ; procedura do sprawdzania obs³ugi rozkazów  AVX

push rbx						   ; zapis wywo³ania procedury do rbx ze stosu, aby jej nie utraciæ

mov r8b, cl						   ; zapisanie poszukiwanej funkcjonalnoœci do r8b

mov eax, 1						  ; nazwa obs³ugiwanej funkcji  przez procesor  przepisywana do rejestru eax

cpuid							  ; wywo³anie  CPUID,  aby ustawiæ odpowiedni bit w rejectrze ecx poszukiwanej funkcjonalnoœci

mov eax, ecx					  ; przenieœ  ustawiony bit odpowiedzi  z ecx do eax, aby go zapamiêtaæ

mov cl, r8b						  ; przenieœ  bit z r8b do cl, aby pó¿niej wykonaæ na cl logiczne przesuniêcie w celu ustawienia rejestru eax na odpowiedni¹ pozycje

shr eax, cl						  ; przesuñ bit odpowiedzi na 0. pozycje rejestru eax 

and eax, 1						  ; zamaskuj pozosta³e bity odpowiedzi na 0, aby wyci¹gn¹æ tylko oczekiwan¹ przez nas infromacje o funkcjonalnoœæi procesora do akumulatora

pop rbx							  ; zwróæ wywo³anie procedury do rbx
								  ; zwróc 1 lub 0 do akumulatora al, gdzie 1 to obs³uga danej funkcjonalnoœci, a 0 to jej brak
ret								  
DetectFeatureECX endp			  ; koniec obs³ugi procedury  sprawdzania osb³ugi rozkazów  AVX
end								  ; koniec progamu
;-------------------------------------------------------------------------