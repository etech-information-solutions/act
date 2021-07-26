import { Component, OnInit } from '@angular/core';
import { PopoverController } from '@ionic/angular';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-confirmdeletesiteaudit',
  templateUrl: './confirmdeletesiteaudit.component.html',
  styleUrls: ['./confirmdeletesiteaudit.component.scss'],
})
export class ConfirmdeletesiteauditComponent implements OnInit
{

  constructor( public auth: UserService, public popCtrl: PopoverController )
  {
    
  }

  ngOnInit()
  {

  }

  Yes()
  {
    this.auth.RemoveSiteAuditConfirmed = true;
    this.popCtrl.dismiss();
  }

  No()
  {
    this.auth.RemoveSiteAuditConfirmed = false;
    this.popCtrl.dismiss();
  }

}
