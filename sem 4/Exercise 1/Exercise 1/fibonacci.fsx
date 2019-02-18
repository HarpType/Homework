let fibonacci n =
    let rec acc_fibonacci n i acc1 acc2 =
            if n = i then
                acc1
            elif n > 0 then
                acc_fibonacci n (i+1) acc2 (acc1+acc2)
            else
                acc_fibonacci n (i-1) (acc2-acc1) acc1
    acc_fibonacci n 0 0 1           
