
;Autor: Micha� Jankowski
; Dzie�: 01.12.2019r.
; Przedmiot: J�zyki Asemblerowe
; Temat: Efekt Sepii


.data
divider_value dq   003000000030000h				 ; sta�a warto�� 3 zapisania w postaci maski w celu wykonania dzielenia wektorowego
alpha_mask dq      0ff000000ff000000h			 ; maska kana�u przezroczysto�ci dla ka�dego piksela
color_R_mask dq    00ff000000ff0000h			 ; maska koloru czerwonego dla ka�dego piksela
.code

Sepia proc									    ; poczatek procedury

								            	; UWAGA w masm x64  rcx,rdx,r9,r10 przechowuj� pierwsze 4 parametry
									            ; je�eli wykorzystujemy wi�cej parametr�w dla procedury musimy je przekaza� przez stos


mov r10, rcx								; �adownie adresu obrazka do rcx

mov r11, rdx								; �adowanie poczatku tablicy do r11 na pozniejsze przetwarzanie

mov rdi, r10								; przenesienie adresu obrazka do rejestru rdi rejestr indeksowy

add rdi, rdx								; dodanie przesuniecia zwaizanego z podzialem na watki

mov rcx, r8									; za�adowanie do rcx ilosci ko�ca tablicy

mov r12, r9									; za�adowanie warto�ci wype�nienia do rejrestru r12
 
sub rcx, rdx								; za�adowanie ilo�ci bit�w do przetworzenia, czyli koniec - poczatek do reejstru rcx 

movlps xmm0, color_R_mask					; �adowanie sta�ej maski koloru R do rejestru xmm0

vinsertf128 ymm0,ymm0, xmm0, 1				; �adowanie sta�ej maski koloru R do  dolnej cz�sci rejestru ymm0 z wykorzystaniem rejestru xmm0

movlps xmm1, divider_value					; �adowanie sta�ego dzielnika (3) do  rejestru xmm1

vinsertf128 ymm1,ymm1, xmm1,1				; �adowanie sta�ego dzielnika (3) do dolnej cz�sci rejestru ymm1 z wykorzystaniem rejestru xmm1

movlps xmm2, alpha_mask						; �adowanie sta�ej maski przezroczysto�ci do rejestru xmm2

vinsertf128 ymm2, ymm2, xmm2, 1				; �adowanie sta�ej maski przezroczysto�ci do  dolnej cz�sci rejestru ymm2 z wykorzystaniem rejestru xmm2

vpshufd ymm0, ymm0, 00h						; ustawianie maski koloru czerwonego w ca�ym obszarze rejestru ymm1, pocz�wszy od pocz�tku rejestru ymm1
vpshufd ymm1, ymm1, 00h						; ustawianie dzielnika w ca�ym obszarze rejestru ymm1, pocz�wszy od pocz�tku rejestru ymm1
vpshufd ymm2, ymm2, 00h						; ustawianie maski kana�u przezroczysto�ci w ca�ym obszarze rejestru ymm1, pocz�wszy od pocz�tku rejestru ymm1
 

vcvtdq2ps ymm3, ymm1					   ; zamiana warto�ci dzielnika 3 z typu int na float w celu wykonania dzielenia wektorowego 

average_loop:							   ; start p�tli wykonuj�cej odcienie szaro�ci
movdqu xmm10, [rdi]						  ; pobranie 4 pikseli z rejestru rdi do xmm10 

add rdi, 16								   ; przesuniecie rejestru indeksowego o 16 pozycji do przodu, w celu pobrania nowych warto�ci z rejestru rdi
sub rcx, 16							       ; przesuniecie rejestru zliczaczj�vego o 16 pozycji do ty�u, aby nie wyj�� poza rejestr

movdqu xmm9, [rdi]							;  pobranie kolejnych 4 pikseli z rejestru rdi do xmm9


vinsertf128 ymm4,ymm4,xmm10,0			   ; przesuniecie 4 pikseli do gornej czesci rejestru ymm4
vinsertf128 ymm4,ymm4,xmm9,1			   ; przsuniecie 4 piskeli do dolnej czesci rejestru ymm4
										   ; przetrzymywanie akutalnie 8 pikseli jednocze�nie w rejestrze ymm4
          
vmovaps ymm6, ymm4						   ; zapamietanie skladowych alpha 8 kolejnych pikseli w rejestrze ymm6


vpand ymm6, ymm6, ymm2					   ; maskowanie kana�u przezroczysto�i 8 kolejnych pikseli w rejestrze ymm6

vmovaps ymm7,ymm4						  ; przepisanie 8 kolejnych pikseli bez kana�u przezroczysto�ci do rejestru ymm7

vpslldq ymm7,  ymm7 , 1					  ; logiczne przesuni�cie rejestru ymm7 o 1 w lewo
vmovaps ymm8, ymm7						  ; przesuni�cie rejstru ymm7 do ymm8
vpslldq  ymm8,ymm8, 1					  ; logiczne przesuni�cie rejestru ymm8 o 1 w lewo

vpand ymm4, ymm4, ymm0					  ; zamaskowanie sk�adowych a, g, b rejestru ymm4 z wykorzystaniem maski kolory czerwonego z rejestru ymm0. Uzyskujemy same kolory czerwone R
vpand ymm7,ymm7,ymm0					  ; zamaskowanie sk�adowych a, r, b rejestru ymm7 z wykorzystaniem maski kolory czerwonego z rejestru ymm0. Uzyskujemy same kolory zielony G
vpand ymm8, ymm8, ymm0					  ; zamaskowanie sk�adowych a, r, b rejestru ymm8 z wykorzystaniem maski kolory czerwonego z rejestru ymm0. Uzyskujemy same kolory zielony B

vpaddd ymm5, ymm4, ymm7					  ; zsumowanie rejestr�w ymm4 i ymm7 do rejestru ymm5. Uzyskujemy sume kolory czerwonego R i zielonego G. R+G
vpaddd ymm5,ymm5, ymm8					  ; zsumowanie rejestr�w ymm5 i ymm8 do rejestru ymm5. Uzyskujemy sume koloru R+G oraz koloru niebieskiego B. R+G+B


										 ; dzielenie wartosci 3 kolejnych skladowcyh pikseli przez wartosc 3
vcvtdq2ps ymm7, ymm5					 ; zamiana warto�ci  int rejestru ymm5 na float do rejestru ymm7 w celu wykonania dzielenia wektorowego
vdivps ymm7, ymm7, ymm3					 ; dzielenie wektorowe rejestru  ymm7 przez rejestr sta�ego dzienika 3 do rejestru ymm7
vcvtps2dq ymm5,ymm7						 ; zamiana warto�ci float rejestru ymm7 na int do reejstru ymm5.

vmovaps ymm7, ymm5						 ; przniesienie reejstru ymm5 do rejestru ymm7

vpslldq ymm5,ymm5, 1					; logiczne przesuni�cie rejestru ymm5 o 1 w celu wpisania warto�ci g, gdzie g = (r + b + g ) / 3 do rejestru: 0,g,g,g
vpor ymm7,ymm7,ymm5						; logiczny OR na rejestrze ymm5 i ymm7 do rejestru ymm7
vpslldq ymm5,ymm5, 1					; logiczne przesuni�cie rejestru ymm5 o 1

										; przepisanie watosci kanalu alpha

vpor ymm7, ymm7, ymm5					; logiczny OR na rejestrze ymm7 i ymm5 w celu przepisanie do rejestru ymm7 warto�ci kana�u alpha
vpor ymm7, ymm7, ymm6					; kolejny logiczny OR na rejestrze ymm7 i ymm5 do rejestru ymm7

vextractf128 xmm10,ymm7,1				; przepisanie dolnej cz�ci rejestru ymm7 do rejestru xmm10 w celu wpisania wynikowych warto�ci

movdqu [rdi], xmm10						; przesuni�cie warto�ci wynikowych pikseli z rejestru xmm10 do rejestru indeksowego rdi, gdzie s� one zapisywane do obrazka


vextractf128 xmm11, ymm7,0			   ; przepisanie g�rnej cz�ci rejestru ymm7 do rejestru xmm11 w celu wpisania wynikowych warto�ci

sub rdi, 16							   ; przesuniecie rejestru indeksowego o 16 pozycji do ty�u umieszczenia pikseli do element�w tablicy obrazka znajduj�cej si� o 16 element�w wcze�niej

movdqu [rdi], xmm11					  ; przesuni�cie warto�ci wynikowych pikseli z rejestru xmm11 do rejestru indeksowego rdi, gdzie s� one zapisywane do obrazka

add rdi, 32							  ; przesuni�cie rdi o 32, aby znajdowa� si� na w�a�ciwej pozycji obrazka. Tutaj r�wnie� ma to zastosowanie  optymalizacyjne.
sub rcx, 16							  ; odj�cie od indeksu zliczaj�cejgo 16, aby nie wyj�� poza koniec tablicy
cmp rcx, 0							  ; sprawdzenie czy przekroczono rozmiar tablicy i zako�czono zliczanie
jle prepare							  ; je�eli  zosta� przekoroczny rozmiar tablicy przygotowujemy do wype�nienia obrazka efektem sepii
jmp average_loop					  ; bezwarunkowy skok do p�tli licz�cej �redni�

prepare:						      ; p�tla przygotowuj�cac do wype�nienia obrazka nadaj�cy efekt sepii
mov rdi, r10						  ; przepisanie do rdi pocz�tku adresu obrazka
add rdi, r11						  ; �adowanie pocz�tku  zliczania element�w tablicy zwi�zanego z podzia�em na w�tki
mov rcx, r8							  ; za�adowanie do rcx ilo�ci bajt�w do przetworzenia
sub rcx, r11						  ; odj�cie przesuni�cia (offsetu) zwi�zanego z podzia�em na w�tki


mov r12, 255					      ; za�adowanie do rejestru r12 warto�ci 255, aby sprawdza� czy nie przekroczono maksymalnej dozwolonej warto�ci piksela
mov r13, r12						  ; przesuni�cie warto�ci rejestru r12 do rejestru r13

              					    
sub r13, r9						    ; odj�cie od rejestru 13 warto�ci spod rejestru r9. 255 - x, gdzie x to warto�� zadanej przez u�ytkownika efektu wype�nienia
sub r13, r9			    		    ; kolejne odj�cie do rejestru r13 warto�ci spod rejestru r9 wynikaj�cego z algorytmu
cmp rax, r13	

ja max_red_t						    ; skok w przypadku przekroczenia maskymalnej warto�ci 255


add rax,r9						    ; je�eli nie przekroczono mmaksymalnej warto�ci dodajemy warto�� wype�nienia do akumulatora raxdla CZERWNOEGO koloru piksela
add rax,r9					        ; drugi raz dodajemy do akumulatora warto�� wype�nienia


jmp continue_red	   		       ; skok bezwarunkowy do etykiety kontynuj�cej wype�nienia koloru CZERWONEGO

max_red_t:				    	   ; etykieta w przypadku przekroczenia maskymalnej warto�ci 255
mov rax, r12	


toneloop:							  ; p�tla wykonuj�ca  wype�nienia do pikseli dla obrazka
mov al, [rdi+1]						  ; przesuni�cie do akumulatora al warto�ci obrazka
mov r13, r12					      ; przesuni�cie do r13 warto�ci 255
sub r13,r9						      ; odj�cie od rejestru 13 warto�ci spod rejestru r9. 255 - x, gdzie x to warto�� zadanej przez u�ytkownika efektu wype�nienia
cmp rax, r13						  ; por�wanie z akumulatorem rax czy przekroczono maksymaln� warto�� 255
ja max_green					      ; skok w przypadku przekroczenia maskymalnej warto�ci 255
add rax, r9						      ; je�eli nie przekroczono mmaksymalnej warto�ci dodajemy warto�� wype�nienia do akumulatora rax dla ZIELONEGO koloru piksela
jmp continue_green					  ; skok do etykiety kontynuj�cej wyp�enienia koloru ZIELONEGO

max_green:							  ; etykieta wpisuj�ca maksymaln� warto�� 255 dla koloru zielonego
mov rax,r12							  ; wpisujemy do  akumulatora rax warto�� 255  z rejestru r12 dla koloru ZIELONEGO

continue_green:					     ; etykieta do kontynowania efektu wype�niania i wsadzenia warto�ci obliczonego wype�nienia do tablicy
mov [rdi+1],al					     ; przesuni�cia warto�ci akumulatora al do adresu obrazka, gdzie akumulator zawiera obliczon� warto�� wype�nienia


mov al,[rdi+2]				    	 ; pobranie kolejnej warto�ci piksela do akumulatora al

mov r13, r12					    ; przesuni�cie do r13 warto�ci 255
sub r13, r9						    ; odj�cie od rejestru 13 warto�ci spod rejestru r9. 255 - x, gdzie x to warto�� zadanej przez u�ytkownika efektu wype�nienia
sub r13, r9			    		    ; kolejne odj�cie do rejestru r13 warto�ci spod rejestru r9 wynikaj�cego z algorytmu
cmp rax, r13					    ; por�wanie z akumulatorem rax czy przekroczono maksymaln� warto�� 255
ja max_red						    ; skok w przypadku przekroczenia maskymalnej warto�ci 255


add rax,r9						    ; je�eli nie przekroczono mmaksymalnej warto�ci dodajemy warto�� wype�nienia do akumulatora raxdla CZERWNOEGO koloru piksela
add rax,r9					        ; drugi raz dodajemy do akumulatora warto�� wype�nienia


jmp continue_red	   		       ; skok bezwarunkowy do etykiety kontynuj�cej wype�nienia koloru CZERWONEGO

max_red:				    	   ; etykieta w przypadku przekroczenia maskymalnej warto�ci 255
mov rax, r12					   ; wpisujemy do  akumulatora rax warto�� 255  z rejestru r12 dla koloru CZERWONOEGO

continue_red:			   		   ; etykieta do kontynowania efektu wype�niania i wsadzenia warto�ci obliczonego wype�nienia do tablicy
mov [rdi+2], al					   ; przesuni�cia warto�ci akumulatora al do adresu obrazka, gdzie akumulator zawiera obliczon� warto�� wype�nienia

add rdi, 4						   ; przesuni�cie o 4 bity do przodu warto�ci  rejestru indeksowego, gdzie znajduj� si� adres obrazka
sub rcx, 4						   ; odj�cie do warto�ci rejestru zliczaj�cego 4, aby nie wyj�� poza zareks tablicy
cmp rcx, 0						   ; sprawdzenie czy zako�czono zliczanie
jle koniec						   ; je�eli warto�� poni�ej zera albo r�wna zeru skocz do etykiety koniec
jmp toneloop					   ; skok bezwarunkowy do p�tli zewn�trznej wykonuj�cej g��wny proces wype�niania obrazka
koniec:							   ; etykieta, w kt�rej ko�czymy wykonywanie naszego przetwarzania obrazka 
ret								   ; powr�t z procedury
Sepia endp						   ; koniec procedury

DetectFeatureEDX proc			   ; procedura do sprawdzania osb�ugi rozkaz�w  MMX

push rbx						   ; zapis wyowa�ania procedury do rbx
mov r8b, cl						   ; zapisanie poszukiwanej funkcjonalno�ci do r8b

mov eax, 1						  ; nazwa obs�ugiwanej funkcji do ecx
cpuid							  ; wywo�aj CPUID
mov eax, edx					  ; przenie�  ustawiony bit odpowiedzi  z edx to eax
mov cl, r8b						  ; przenie�  bit z r8b do cl, aby p�niej wykona� na cl logiczne przesuni�cie

shr eax, cl						  ; przesu� bit odpowiedzi na 0. pozycje rejestru eax 
and eax, 1						  ; zamaskuj pozosta�e bity odpowiedzi na 0
pop rbx							  ; zwr�� wywo�anie procedury do rbx
ret								  ; zwr�c 1 lub 0 do akumulatora al
DetectFeatureEDX endp             ; koniec obs�ugi procedury  sprawdzania osb�ugi rozkaz�w  MMX

DetectFeatureECX proc			  ; procedura do sprawdzania obs�ugi rozkaz�w  AVX

push rbx						   ; zapis wyowa�ania procedury do rbx
mov r8b, cl						   ; zapisanie poszukiwanej funkcjonalno�ci do r8b

mov eax, 1						  ; nazwa obs�ugiwanej funkcji do ecx
cpuid							  ; wywo�aj CPUID
mov eax, ecx					  ; przenie�  ustawiony bit odpowiedzi  z ecx to eax
mov cl, r8b						  ; przenie�  bit z r8b do cl, aby p�niej wykona� na cl logiczne przesuni�cie

shr eax, cl						  ; przesu� bit odpowiedzi na 0. pozycje rejestru eax 
and eax, 1						  ; zamaskuj pozosta�e bity odpowiedzi na 0
pop rbx							  ; zwr�� wywo�anie procedury do rbx
ret								  ; zwr�c 1 lub 0 do akumulatora al
DetectFeatureECX endp			  ; koniec obs�ugi procedury  sprawdzania osb�ugi rozkaz�w  AVX
end ; koniec progamu
;-------------------------------------------------------------------------