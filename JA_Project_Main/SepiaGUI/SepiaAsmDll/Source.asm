
;Autor: Micha³ Jankowski
; Dzieñ: 01.12.2019r.
; Przedmiot: Jêzyki Asemblerowe
; Temat: Efekt Sepii


.data
divider_value dq   003000000030000h				 ; sta³a wartoœæ 3 zapisania w postaci maski w celu wykonania dzielenia wektorowego
alpha_mask dq      0ff000000ff000000h			 ; maska kana³u przezroczystoœci dla ka¿dego piksela
color_R_mask dq    00ff000000ff0000h			 ; maska koloru czerwonego dla ka¿dego piksela
.code

Sepia proc									    ; poczatek procedury

								            	; UWAGA w masm x64  rcx,rdx,r9,r10 przechowuj¹ pierwsze 4 parametry
									            ; je¿eli wykorzystujemy wiêcej parametrów dla procedury musimy je przekazaæ przez stos


mov r10, rcx								; ³adownie adresu obrazka do rcx

mov r11, rdx								; ³adowanie poczatku tablicy do r11 na pozniejsze przetwarzanie

mov rdi, r10								; przenesienie adresu obrazka do rejestru rdi rejestr indeksowy

add rdi, rdx								; dodanie przesuniecia zwaizanego z podzialem na watki

mov rcx, r8									; za³adowanie do rcx ilosci koñca tablicy

mov r12, r9									; za³adowanie wartoœci wype³nienia do rejrestru r12
 
sub rcx, rdx								; za³adowanie iloœci bitów do przetworzenia, czyli koniec - poczatek do reejstru rcx 

movlps xmm0, color_R_mask					; ³adowanie sta³ej maski koloru R do rejestru xmm0

vinsertf128 ymm0,ymm0, xmm0, 1				; ³adowanie sta³ej maski koloru R do  dolnej czêsci rejestru ymm0 z wykorzystaniem rejestru xmm0

movlps xmm1, divider_value					; ³adowanie sta³ego dzielnika (3) do  rejestru xmm1

vinsertf128 ymm1,ymm1, xmm1,1				; ³adowanie sta³ego dzielnika (3) do dolnej czêsci rejestru ymm1 z wykorzystaniem rejestru xmm1

movlps xmm2, alpha_mask						; ³adowanie sta³ej maski przezroczystoœci do rejestru xmm2

vinsertf128 ymm2, ymm2, xmm2, 1				; ³adowanie sta³ej maski przezroczystoœci do  dolnej czêsci rejestru ymm2 z wykorzystaniem rejestru xmm2

vpshufd ymm0, ymm0, 00h						; ustawianie maski koloru czerwonego w ca³ym obszarze rejestru ymm1, pocz¹wszy od pocz¹tku rejestru ymm1
vpshufd ymm1, ymm1, 00h						; ustawianie dzielnika w ca³ym obszarze rejestru ymm1, pocz¹wszy od pocz¹tku rejestru ymm1
vpshufd ymm2, ymm2, 00h						; ustawianie maski kana³u przezroczystoœci w ca³ym obszarze rejestru ymm1, pocz¹wszy od pocz¹tku rejestru ymm1
 

vcvtdq2ps ymm3, ymm1					   ; zamiana wartoœci dzielnika 3 z typu int na float w celu wykonania dzielenia wektorowego 

average_loop:							   ; start pêtli wykonuj¹cej odcienie szaroœci
movdqu xmm10, [rdi]						  ; pobranie 4 pikseli z rejestru rdi do xmm10 

add rdi, 16								   ; przesuniecie rejestru indeksowego o 16 pozycji do przodu, w celu pobrania nowych wartoœci z rejestru rdi
sub rcx, 16							       ; przesuniecie rejestru zliczaczj¹vego o 16 pozycji do ty³u, aby nie wyjœæ poza rejestr

movdqu xmm9, [rdi]							;  pobranie kolejnych 4 pikseli z rejestru rdi do xmm9


vinsertf128 ymm4,ymm4,xmm10,0			   ; przesuniecie 4 pikseli do gornej czesci rejestru ymm4
vinsertf128 ymm4,ymm4,xmm9,1			   ; przsuniecie 4 piskeli do dolnej czesci rejestru ymm4
										   ; przetrzymywanie akutalnie 8 pikseli jednoczeœnie w rejestrze ymm4
          
vmovaps ymm6, ymm4						   ; zapamietanie skladowych alpha 8 kolejnych pikseli w rejestrze ymm6


vpand ymm6, ymm6, ymm2					   ; maskowanie kana³u przezroczystoæi 8 kolejnych pikseli w rejestrze ymm6

vmovaps ymm7,ymm4						  ; przepisanie 8 kolejnych pikseli bez kana³u przezroczystoœci do rejestru ymm7

vpslldq ymm7,  ymm7 , 1					  ; logiczne przesuniêcie rejestru ymm7 o 1 w lewo
vmovaps ymm8, ymm7						  ; przesuniêcie rejstru ymm7 do ymm8
vpslldq  ymm8,ymm8, 1					  ; logiczne przesuniêcie rejestru ymm8 o 1 w lewo

vpand ymm4, ymm4, ymm0					  ; zamaskowanie sk³adowych a, g, b rejestru ymm4 z wykorzystaniem maski kolory czerwonego z rejestru ymm0. Uzyskujemy same kolory czerwone R
vpand ymm7,ymm7,ymm0					  ; zamaskowanie sk³adowych a, r, b rejestru ymm7 z wykorzystaniem maski kolory czerwonego z rejestru ymm0. Uzyskujemy same kolory zielony G
vpand ymm8, ymm8, ymm0					  ; zamaskowanie sk³adowych a, r, b rejestru ymm8 z wykorzystaniem maski kolory czerwonego z rejestru ymm0. Uzyskujemy same kolory zielony B

vpaddd ymm5, ymm4, ymm7					  ; zsumowanie rejestrów ymm4 i ymm7 do rejestru ymm5. Uzyskujemy sume kolory czerwonego R i zielonego G. R+G
vpaddd ymm5,ymm5, ymm8					  ; zsumowanie rejestrów ymm5 i ymm8 do rejestru ymm5. Uzyskujemy sume koloru R+G oraz koloru niebieskiego B. R+G+B


										 ; dzielenie wartosci 3 kolejnych skladowcyh pikseli przez wartosc 3
vcvtdq2ps ymm7, ymm5					 ; zamiana wartoœci  int rejestru ymm5 na float do rejestru ymm7 w celu wykonania dzielenia wektorowego
vdivps ymm7, ymm7, ymm3					 ; dzielenie wektorowe rejestru  ymm7 przez rejestr sta³ego dzienika 3 do rejestru ymm7
vcvtps2dq ymm5,ymm7						 ; zamiana wartoœci float rejestru ymm7 na int do reejstru ymm5.

vmovaps ymm7, ymm5						 ; przniesienie reejstru ymm5 do rejestru ymm7

vpslldq ymm5,ymm5, 1					; logiczne przesuniêcie rejestru ymm5 o 1 w celu wpisania wartoœci g, gdzie g = (r + b + g ) / 3 do rejestru: 0,g,g,g
vpor ymm7,ymm7,ymm5						; logiczny OR na rejestrze ymm5 i ymm7 do rejestru ymm7
vpslldq ymm5,ymm5, 1					; logiczne przesuniêcie rejestru ymm5 o 1

										; przepisanie watosci kanalu alpha

vpor ymm7, ymm7, ymm5					; logiczny OR na rejestrze ymm7 i ymm5 w celu przepisanie do rejestru ymm7 wartoœci kana³u alpha
vpor ymm7, ymm7, ymm6					; kolejny logiczny OR na rejestrze ymm7 i ymm5 do rejestru ymm7

vextractf128 xmm10,ymm7,1				; przepisanie dolnej czêœci rejestru ymm7 do rejestru xmm10 w celu wpisania wynikowych wartoœci

movdqu [rdi], xmm10						; przesuniêcie wartoœci wynikowych pikseli z rejestru xmm10 do rejestru indeksowego rdi, gdzie s¹ one zapisywane do obrazka


vextractf128 xmm11, ymm7,0			   ; przepisanie górnej czêœci rejestru ymm7 do rejestru xmm11 w celu wpisania wynikowych wartoœci

sub rdi, 16							   ; przesuniecie rejestru indeksowego o 16 pozycji do ty³u umieszczenia pikseli do elementów tablicy obrazka znajduj¹cej siê o 16 elementów wczeœniej

movdqu [rdi], xmm11					  ; przesuniêcie wartoœci wynikowych pikseli z rejestru xmm11 do rejestru indeksowego rdi, gdzie s¹ one zapisywane do obrazka

add rdi, 32							  ; przesuniêcie rdi o 32, aby znajdowaæ siê na w³aœciwej pozycji obrazka. Tutaj równie¿ ma to zastosowanie  optymalizacyjne.
sub rcx, 16							  ; odjêcie od indeksu zliczaj¹cejgo 16, aby nie wyjœæ poza koniec tablicy
cmp rcx, 0							  ; sprawdzenie czy przekroczono rozmiar tablicy i zakoñczono zliczanie
jle prepare							  ; je¿eli  zosta³ przekoroczny rozmiar tablicy przygotowujemy do wype³nienia obrazka efektem sepii
jmp average_loop					  ; bezwarunkowy skok do pêtli licz¹cej œredni¹

prepare:						      ; pêtla przygotowuj¹cac do wype³nienia obrazka nadaj¹cy efekt sepii
mov rdi, r10						  ; przepisanie do rdi pocz¹tku adresu obrazka
add rdi, r11						  ; ³adowanie pocz¹tku  zliczania elementów tablicy zwi¹zanego z podzia³em na w¹tki
mov rcx, r8							  ; za³adowanie do rcx iloœci bajtów do przetworzenia
sub rcx, r11						  ; odjêcie przesuniêcia (offsetu) zwi¹zanego z podzia³em na w¹tki


mov r12, 255					      ; za³adowanie do rejestru r12 wartoœci 255, aby sprawdzaæ czy nie przekroczono maksymalnej dozwolonej wartoœci piksela
mov r13, r12						  ; przesuniêcie wartoœci rejestru r12 do rejestru r13

              					    
sub r13, r9						    ; odjêcie od rejestru 13 wartoœci spod rejestru r9. 255 - x, gdzie x to wartoœæ zadanej przez u¿ytkownika efektu wype³nienia
sub r13, r9			    		    ; kolejne odjêcie do rejestru r13 wartoœci spod rejestru r9 wynikaj¹cego z algorytmu
cmp rax, r13	

ja max_red_t						    ; skok w przypadku przekroczenia maskymalnej wartoœci 255


add rax,r9						    ; je¿eli nie przekroczono mmaksymalnej wartoœci dodajemy wartoœæ wype³nienia do akumulatora raxdla CZERWNOEGO koloru piksela
add rax,r9					        ; drugi raz dodajemy do akumulatora wartoœæ wype³nienia


jmp continue_red	   		       ; skok bezwarunkowy do etykiety kontynuj¹cej wype³nienia koloru CZERWONEGO

max_red_t:				    	   ; etykieta w przypadku przekroczenia maskymalnej wartoœci 255
mov rax, r12	


toneloop:							  ; pêtla wykonuj¹ca  wype³nienia do pikseli dla obrazka
mov al, [rdi+1]						  ; przesuniêcie do akumulatora al wartoœci obrazka
mov r13, r12					      ; przesuniêcie do r13 wartoœci 255
sub r13,r9						      ; odjêcie od rejestru 13 wartoœci spod rejestru r9. 255 - x, gdzie x to wartoœæ zadanej przez u¿ytkownika efektu wype³nienia
cmp rax, r13						  ; porówanie z akumulatorem rax czy przekroczono maksymaln¹ wartoœæ 255
ja max_green					      ; skok w przypadku przekroczenia maskymalnej wartoœci 255
add rax, r9						      ; je¿eli nie przekroczono mmaksymalnej wartoœci dodajemy wartoœæ wype³nienia do akumulatora rax dla ZIELONEGO koloru piksela
jmp continue_green					  ; skok do etykiety kontynuj¹cej wyp³enienia koloru ZIELONEGO

max_green:							  ; etykieta wpisuj¹ca maksymaln¹ wartoœæ 255 dla koloru zielonego
mov rax,r12							  ; wpisujemy do  akumulatora rax wartoœæ 255  z rejestru r12 dla koloru ZIELONEGO

continue_green:					     ; etykieta do kontynowania efektu wype³niania i wsadzenia wartoœci obliczonego wype³nienia do tablicy
mov [rdi+1],al					     ; przesuniêcia wartoœci akumulatora al do adresu obrazka, gdzie akumulator zawiera obliczon¹ wartoœæ wype³nienia


mov al,[rdi+2]				    	 ; pobranie kolejnej wartoœci piksela do akumulatora al

mov r13, r12					    ; przesuniêcie do r13 wartoœci 255
sub r13, r9						    ; odjêcie od rejestru 13 wartoœci spod rejestru r9. 255 - x, gdzie x to wartoœæ zadanej przez u¿ytkownika efektu wype³nienia
sub r13, r9			    		    ; kolejne odjêcie do rejestru r13 wartoœci spod rejestru r9 wynikaj¹cego z algorytmu
cmp rax, r13					    ; porówanie z akumulatorem rax czy przekroczono maksymaln¹ wartoœæ 255
ja max_red						    ; skok w przypadku przekroczenia maskymalnej wartoœci 255


add rax,r9						    ; je¿eli nie przekroczono mmaksymalnej wartoœci dodajemy wartoœæ wype³nienia do akumulatora raxdla CZERWNOEGO koloru piksela
add rax,r9					        ; drugi raz dodajemy do akumulatora wartoœæ wype³nienia


jmp continue_red	   		       ; skok bezwarunkowy do etykiety kontynuj¹cej wype³nienia koloru CZERWONEGO

max_red:				    	   ; etykieta w przypadku przekroczenia maskymalnej wartoœci 255
mov rax, r12					   ; wpisujemy do  akumulatora rax wartoœæ 255  z rejestru r12 dla koloru CZERWONOEGO

continue_red:			   		   ; etykieta do kontynowania efektu wype³niania i wsadzenia wartoœci obliczonego wype³nienia do tablicy
mov [rdi+2], al					   ; przesuniêcia wartoœci akumulatora al do adresu obrazka, gdzie akumulator zawiera obliczon¹ wartoœæ wype³nienia

add rdi, 4						   ; przesuniêcie o 4 bity do przodu wartoœci  rejestru indeksowego, gdzie znajdujê siê adres obrazka
sub rcx, 4						   ; odjêcie do wartoœci rejestru zliczaj¹cego 4, aby nie wyjœæ poza zareks tablicy
cmp rcx, 0						   ; sprawdzenie czy zakoñczono zliczanie
jle koniec						   ; je¿eli wartoœæ poni¿ej zera albo równa zeru skocz do etykiety koniec
jmp toneloop					   ; skok bezwarunkowy do pêtli zewnêtrznej wykonuj¹cej g³ówny proces wype³niania obrazka
koniec:							   ; etykieta, w której koñczymy wykonywanie naszego przetwarzania obrazka 
ret								   ; powrót z procedury
Sepia endp						   ; koniec procedury

DetectFeatureEDX proc			   ; procedura do sprawdzania osb³ugi rozkazów  MMX

push rbx						   ; zapis wyowa³ania procedury do rbx
mov r8b, cl						   ; zapisanie poszukiwanej funkcjonalnoœci do r8b

mov eax, 1						  ; nazwa obs³ugiwanej funkcji do ecx
cpuid							  ; wywo³aj CPUID
mov eax, edx					  ; przenieœ  ustawiony bit odpowiedzi  z edx to eax
mov cl, r8b						  ; przenieœ  bit z r8b do cl, aby pó¿niej wykonaæ na cl logiczne przesuniêcie

shr eax, cl						  ; przesuñ bit odpowiedzi na 0. pozycje rejestru eax 
and eax, 1						  ; zamaskuj pozosta³e bity odpowiedzi na 0
pop rbx							  ; zwróæ wywo³anie procedury do rbx
ret								  ; zwróc 1 lub 0 do akumulatora al
DetectFeatureEDX endp             ; koniec obs³ugi procedury  sprawdzania osb³ugi rozkazów  MMX

DetectFeatureECX proc			  ; procedura do sprawdzania obs³ugi rozkazów  AVX

push rbx						   ; zapis wyowa³ania procedury do rbx
mov r8b, cl						   ; zapisanie poszukiwanej funkcjonalnoœci do r8b

mov eax, 1						  ; nazwa obs³ugiwanej funkcji do ecx
cpuid							  ; wywo³aj CPUID
mov eax, ecx					  ; przenieœ  ustawiony bit odpowiedzi  z ecx to eax
mov cl, r8b						  ; przenieœ  bit z r8b do cl, aby pó¿niej wykonaæ na cl logiczne przesuniêcie

shr eax, cl						  ; przesuñ bit odpowiedzi na 0. pozycje rejestru eax 
and eax, 1						  ; zamaskuj pozosta³e bity odpowiedzi na 0
pop rbx							  ; zwróæ wywo³anie procedury do rbx
ret								  ; zwróc 1 lub 0 do akumulatora al
DetectFeatureECX endp			  ; koniec obs³ugi procedury  sprawdzania osb³ugi rozkazów  AVX
end ; koniec progamu
;-------------------------------------------------------------------------